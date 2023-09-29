using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.SqlServer;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using Coderr.Server.WebSite.Infrastructure.Cqs.Adapters;
using DotNetCqs;
using DotNetCqs.Bus;
using DotNetCqs.DependencyInjection;
using DotNetCqs.MessageProcessor;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Coderr.Server.WebSite.Infrastructure.Cqs
{
    public class RegisterCqsServices : IAppModule
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RegisterCqsServices));
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private QueueListener _queueListener;
        private Task _queueListenerTask;

        public void Configure(ConfigurationContext context)
        {
            var assembly = typeof(IAccountService).Assembly;
            context.Services.RegisterMessageHandlers(assembly);

            assembly = typeof(SqlServerTools).Assembly;
            context.Services.RegisterMessageHandlers(assembly);

            context.Services.AddScoped<ScopeCommitter>();
            context.Services.AddScoped<ExecuteDirectlyMessageBus>();
            context.Services.AddSingleton(QueueManager.Instance.QueueProvider);
            context.Services.AddSingleton<IRouteRegistrar>(MessageRouter.Instance);
            context.Services.AddSingleton<IMessageRouter>(MessageRouter.Instance);

            context.Services.AddSingleton<IMessageBus>(x =>
            {
                var appQueueName =
                    ServerConfig.Instance.IsLive
                        ? context.Configuration.GetSection("MessageQueue")["AppQueue"]
                        : "Messaging";
                var appQueue = x.GetService<IMessageQueueProvider>().Open(appQueueName);

                var reportQueueName =
                    ServerConfig.Instance.IsLive
                        ? context.Configuration.GetSection("MessageQueue")["ReportQueue"]
                        : "ErrorReports";
                var reportQueue = x.GetService<IMessageQueueProvider>().Open(reportQueueName);

                MessageRouter.Instance.RegisterAppQueue(appQueue);
                MessageRouter.Instance.RegisterReportQueue(reportQueue);
                return new RoutingMessageBus(MessageRouter.Instance);
            });

            context.Services.AddScoped<IQueryBus, ScopedQueryBus>();
            context.Services.AddScoped(CreateMessageInvoker);

            _log.Debug("Creating listeners..");
            _queueListener = CreateQueueListener(context);
        }

        private QueueListener CreateQueueListener(ConfigurationContext context)
        {
            if (ServerConfig.Instance.IsLive)
            {
                var appQueueName = context.Configuration.GetSection("MessageQueue")["AppQueue"];
                var reportQueueName = context.Configuration.GetSection("MessageQueue")["ReportQueue"];
                return ConfigureQueueListener(context, appQueueName, appQueueName, reportQueueName);
            }

            return ConfigureQueueListener(context, "Messaging", "Messaging", "ErrorReports");
        }

        private QueueListener ConfigureQueueListener(ConfigurationContext context, string inboundQueueName, string outboundQueueName, string reportAnalyzerQueue)
        {
            var inboundQueue = QueueManager.Instance.GetQueue(inboundQueueName);
            var outboundQueue = inboundQueueName == outboundQueueName
                ? inboundQueue
                : QueueManager.Instance.GetQueue(outboundQueueName);
            var scopeFactory = new ScopeFactory(context.ServiceProvider);

            MessageRouter.Instance.RegisterAppQueue(outboundQueue);
            MessageRouter.Instance.RegisterReportQueue(QueueManager.Instance.GetQueue(reportAnalyzerQueue));

            _log.Debug($"Loading listener {inboundQueueName} with outbound {outboundQueueName} and report queue {reportAnalyzerQueue}.");
            var listener = new QueueListener(inboundQueue, MessageRouter.Instance, scopeFactory)
            {
                RetryAttempts = new[]
                    {TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},
                MessageInvokerFactory = scope => new MessageInvoker(scope),
            };
            listener.PoisonMessageDetected += (sender, args) =>
            {
                //Err.Report(args.Exception, new { args.Message });
                _log.Error($"{inboundQueueName} Poison message: {args.Message.Body}", args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                _log.Debug(
                    $"CoreApp {inboundQueueName} scope created: {args.Scope.GetHashCode()} for {args.Message.Body.ToString().Replace("\r\n", " ")}.");
                var accessor = args.Scope.ResolveDependency<IPrincipalAccessor>().First();
                accessor.Principal = args.Principal;
                _log.Info(
                    $"CoreApp {inboundQueueName} scope: {args.Scope.GetHashCode()} mapped to {accessor.GetHashCode()}, Credentials: {args.Principal.ToFriendlyString()}.");
            };
            listener.ScopeClosing += (sender, args) =>
            {
                if (args.Exception != null)
                    return;

                _log.Debug($"CoreApp {inboundQueueName} {args.Scope.GetHashCode()} Saving changes now for {args.Message.Body}");
                var all = args.Scope.ResolveDependency<IAdoNetUnitOfWork>().ToList();
                all[0].SaveChanges();
            };
            listener.MessageInvokerFactory = MessageInvokerFactory;
            return listener;
        }

        public void Start(StartContext context)
        {
            _queueListenerTask = _queueListener.RunAsync(_cancelTokenSource.Token);
            _queueListenerTask.ContinueWith(OnListenerStopped);
        }

        private void OnListenerStopped(Task obj)
        {


        }

        public void Stop()
        {
            try
            {
                _log.Debug("Shutting down CQS listeners.");
                _cancelTokenSource.Cancel();
                _queueListenerTask.Wait(5000);
            }
            catch (TaskCanceledException)
            {
                _log.Debug("Task has been canceled1.");
            }
            catch (Exception ex)
            {
                if (ex is AggregateException ae && ae.InnerException is TaskCanceledException)
                {
                    _log.Debug("Task has been canceled.");
                }
                else
                {
                    _log.Error("Failed to wait 10s.", ex);
                }
            }

            _log.Debug("Shutting down CQS listeners was successful.");
        }

        private IMessageInvoker CreateMessageInvoker(IServiceProvider x)
        {
            var invoker = new MessageInvoker(new HandlerScopeWrapper(x));
            invoker.HandlerMissing += (sender, args) =>
            {
                if (args.Message.Body is IncidentCreated)
                    return;

                _log.Error("CoreCqs No handler for " + args.Message.Body.GetType());
                try
                {
                    throw new NoHandlerRegisteredException(
                        "Failed to find a handler for " + args.Message.Body.GetType());
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new { args.Message.Body });
                }
            };

            invoker.InvokingHandler += (sender, args) =>
            {
                _log.Debug(
                    $"CoreCqs scope {args.Scope.GetHashCode()} Invoking {JsonConvert.SerializeObject(args.Message)} ({args.Handler.GetType()}).");
            };
            invoker.HandlerInvoked += (sender, args) =>
            {
                if (args.Exception == null)
                    return;

                var closer = args.Scope.ResolveDependency<ScopeCommitter>().First();
                closer.Exception = args.Exception;

                _log.Error(
                    $"CoreCqs Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED, principal: " + args.Principal.ToFriendlyString(),
                    args.Exception);

                Err.Report(args.Exception, new
                {
                    args.Message.Body,
                    HandlerType = args.Handler.GetType(),
                    args.ExecutionTime
                });
            };
            return invoker;
        }

        private IMessageInvoker MessageInvokerFactory(IHandlerScope arg)
        {
            var invoker = new MessageInvoker(arg);
            invoker.HandlerMissing += (sender, args) =>
            {
                _log.Warn("Handler missing for " + args.Message.Body.GetType());
            };
            invoker.HandlerInvoked += (sender, args) =>
            {
                _log.Debug(args.Message.Body);
                if (args.Exception == null)
                    return;

                Err.Report(args.Exception, new
                {
                    args.Message.Body,
                    HandlerType = args.Handler.GetType(),
                    args.ExecutionTime
                });
                _log.Error(
                    $"Ran {args.Handler} with {args.Message.Body}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED, principal: " + args.Principal.ToFriendlyString(),
                    args.Exception);
            };
            return invoker;
        }

        public static bool IsQuery(object cqsObject)
        {
            if (cqsObject == null) throw new ArgumentNullException(nameof(cqsObject));
            var baseType = cqsObject.GetType().BaseType;
            while (baseType != null)
            {
                if (baseType.FullName.StartsWith("DotNetCqs.Query"))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}