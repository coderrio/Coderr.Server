using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using DotNetCqs.Bus;
using DotNetCqs.DependencyInjection;
using DotNetCqs.Logging;
using DotNetCqs.MessageProcessor;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    internal class ReportQueueModule : IReportAnalyzerModule
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportQueueModule));
        private QueueListener _eventProcessor;
        private QueueListener _reportListener;

        public ReportQueueModule()
        {
        }

        public void Configure(ConfigurationContext context)
        {
            _logger.Debug("configuring on  " + context.GetHashCode());
            ConfigureListeners(context);
            ConfigureMessageHandlers(context);

            if (!ServerConfig.Instance.IsLive)
                CreateDomainQueue(context, "Messaging");
        }

        public void Start(StartContext context)
        {
            //These should not be blocking.But they are working as blocking. quite strange.


            if (_reportListener != null)
                ThreadPool.QueueUserWorkItem(state =>
                {
                    _reportListener.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
                });

            if (_eventProcessor != null)
                ThreadPool.QueueUserWorkItem(state =>
                {
                    _eventProcessor.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
                });
        }

        public void Stop()
        {
            _logger.Info("Report queue module is shutting down.");
            _cancellationTokenSource.Cancel();
        }

        private void ConfigureListeners(ConfigurationContext context)
        {
            if (Environment.GetEnvironmentVariable("DisableReportQueue") == "1")
                return;

            var reportQueueName = ServerConfig.Instance.Queues.ReportQueue;
            var reportEventQueue = ServerConfig.Instance.Queues.ReportEventQueue;
            var appQueue = ServerConfig.Instance.Queues.AppQueue;

            var typeName = ServerConfig.Instance.IsLive ? "LIVE" : "PREMISE";
            _logger.Info($"Running AS  {typeName} with queues {reportQueueName}, {appQueue} and {reportEventQueue}");
            _reportListener = ConfigureQueueListener(context, reportQueueName, reportEventQueue, appQueue);
            _eventProcessor = ConfigureQueueListener(context, reportEventQueue, reportEventQueue, appQueue);
        }

        private void ConfigureMessageHandlers(ConfigurationContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            context.Services.RegisterMessageHandlers(assembly);

            //workaround since SQL server already references us
            assembly = AppDomain.CurrentDomain.GetAssemblies()
                .First(x => x.FullName.StartsWith("Coderr.Server.SqlServer,"));
            context.Services.RegisterMessageHandlers(assembly);
        }

        private QueueListener ConfigureQueueListener(ConfigurationContext context, string inboundQueueName,
            string outboundQueueName, string appQueue)
        {
            var inboundQueue = QueueManager.Instance.GetQueue(inboundQueueName);
            var outboundQueue = inboundQueueName == outboundQueueName
                ? inboundQueue
                : QueueManager.Instance.GetQueue(outboundQueueName);
            var scopeFactory = new ScopeWrapper(context.ServiceProvider);

            MessageRouter.Instance.RegisterReportQueue(outboundQueue);
            MessageRouter.Instance.RegisterAppQueue(QueueManager.Instance.GetQueue(appQueue));

            var listener = new QueueListener(inboundQueue, MessageRouter.Instance, scopeFactory)
            {
                RetryAttempts =
                    new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) },
                MessageInvokerFactory = scope =>
                {
                    var invoker = new MessageInvoker(scope);
                    invoker.InvokingHandler += (sender, args) =>
                    {
                        _logger.Debug("Invoking " + args.Handler.GetType().FullName);
                    };
                    invoker.HandlerMissing += (sender, args) =>
                    {
                        _logger.Error("Missing handler for " + args.Message.Body.GetType().FullName);
                    };
                    return invoker;
                },
            };
            listener.PoisonMessageDetected += (sender, args) =>
            {
                Err.Report(args.Exception, new { args.Message.Body });
                _logger.Error($"CoreReport [{inboundQueueName}] Poison message: {args.Message.Body}", args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                _logger.Debug($"CoreReport [{inboundQueueName} principal " + args.Principal.ToLogString());
                var accessor = args.Scope.ResolveDependency<IPrincipalAccessor>().First();
                accessor.Principal = args.Principal;
            };
            listener.ScopeClosing += (sender, args) =>
            {
                if (args.Exception != null)
                    return;

                var all = args.Scope.ResolveDependency<IAdoNetUnitOfWork>().ToList();
                all[0].SaveChanges();

                var queue = (ISaveable)args.Scope.ResolveDependency<IDomainQueue>().First();
                queue.SaveChanges().GetAwaiter().GetResult();
            };
            listener.MessageInvokerFactory = MessageInvokerFactory;
            return listener;
        }

        /// <summary>
        ///     Writes to the message queue that the application is processing (publishing in the other bounded context)
        /// </summary>
        /// <param name="context"></param>
        private void CreateDomainQueue(ConfigurationContext context, string queueName)
        {
            context.Services.AddScoped<IDomainQueue>(x =>
            {
                var queue = QueueManager.Instance.GetQueue(queueName);
                var messageBus = new ScopedMessageBus(queue);
                return new DomainQueueWrapper3(messageBus);
            });
        }

        private void HaveRun(Task obj)
        {
            _logger.Info("Stop completed for a listener in the ReportQueueModule. " + obj);
        }

        private IMessageInvoker MessageInvokerFactory(IHandlerScope arg)
        {
            var invoker = new MessageInvoker(arg);
            invoker.HandlerMissing += (sender, args) =>
            {
                if (args.Message.Body is IncidentCreated)
                    return;

                _logger.Warn(
                    "Failed to find a handler for " + args.Message.Body.GetType());
            };
            invoker.InvokingHandler += (sender, args) =>
            {
                var asseccor = arg.ResolveDependency<IPrincipalAccessor>().FirstOrDefault();
                asseccor.Principal = args.Principal;
            };

            invoker.HandlerInvoked += (sender, args) =>
            {
                if (args.ExecutionTime.TotalMilliseconds > 500)
                {

                    _logger.Error(
                        $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms. Credentials: {args.Principal.ToFriendlyString()}");
                }
                if (args.Exception == null)
                    return;

                //Err.Report(args.Exception, new
                //{
                //    args.Message,
                //    HandlerType = args.Handler.GetType(),
                //    args.ExecutionTime
                //});
                _logger.Error(
                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED. Credentials: {args.Principal.ToFriendlyString()}",
                    args.Exception);
            };
            return invoker;
        }
    }
}