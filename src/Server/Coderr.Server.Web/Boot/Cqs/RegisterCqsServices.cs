using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using codeRR.Client;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.Infrastructure.Security;
using Coderr.Server.SqlServer;
using Coderr.Server.Web2.Boot.Adapters;
using DotNetCqs;
using DotNetCqs.Bus;
using DotNetCqs.DependencyInjection;
using DotNetCqs.MessageProcessor;
using DotNetCqs.Queues;
using DotNetCqs.Queues.AdoNet;
using Griffin.Data;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Coderr.Server.Web2.Boot.Cqs
{
    public class RegisterCqsServices : ISystemModule
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RegisterCqsServices));
        private readonly ClaimsPrincipal _systemPrincipal;
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private QueueListener _messagingQueueListener;
        private Task _messagingQueueListenerTask;
        private AdoNetMessageQueueProvider _queueProvider;

        public RegisterCqsServices()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, CoderrClaims.RoleSystem),
                new Claim(ClaimTypes.Name, "System")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _systemPrincipal = principal;
        }


        public void Configure(ConfigurationContext context)
        {
            var assembly = typeof(IAccountService).Assembly;
            ConfigureAssembly(assembly, context);

            assembly = typeof(SqlServerTools).Assembly;
            ConfigureAssembly(assembly, context);

            context.Services.AddSingleton<IMessageQueueProvider>(CreateQueueProvider(context));
            context.Services.AddSingleton<IMessageBus>(x =>
            {
                var queue = x.GetService<IMessageQueueProvider>().Open("Messaging");
                var bus = new SingleInstanceMessageBus(queue);
                return bus;
            });
            context.Services.AddScoped<IQueryBus, ScopedQueryBus>();
            context.Services.AddScoped(CreateMessageInvoker);

            _messagingQueueListener = ConfigureQueueListener(context, "Messaging", "Messaging");
        }

        public void Start(StartContext context)
        {
            _messagingQueueListener.RunAsync(_cancelTokenSource.Token).ContinueWith(OnListenerStopped);
        }

        private void OnListenerStopped(Task obj)
        {
            

        }

        public void Stop()
        {
            _cancelTokenSource.Cancel();
            _messagingQueueListenerTask.Wait(10000);
        }


        private void ConfigureAssembly(Assembly assembly, ConfigurationContext context)
        {
            foreach (var type in assembly.GetTypes())
            {
                var ifs = type.GetInterfaces();
                var ourInterface = ifs.FirstOrDefault(x => x.Name.Contains("QueryHandler"))
                                   ?? ifs.FirstOrDefault(x => x.Name.Contains("MessageHandler"));
                if (ourInterface != null)
                    context.Services.AddScoped(ourInterface, type);
            }
        }

        private QueueListener ConfigureQueueListener(ConfigurationContext context, string inboundQueueName,
            string outboundQueueName)
        {
            var inboundQueue = _queueProvider.Open(inboundQueueName);
            var outboundQueue = inboundQueueName == outboundQueueName
                ? inboundQueue
                : _queueProvider.Open(outboundQueueName);
            var scopeFactory = new ScopeWrapper(context.ServiceProvider);
            var listener = new QueueListener(inboundQueue, outboundQueue, scopeFactory)
            {
                RetryAttempts = new[]
                    {TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},
                MessageInvokerFactory = scope => new MessageInvoker(scope),
                Logger = DiagnosticLog
            };
            listener.PoisonMessageDetected += (sender, args) =>
            {
                //Err.Report(args.Exception, new { args.Message });
                _log.Error(inboundQueueName + " Poison message: " + args.Message.Body, args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                _log.Debug(inboundQueueName + " Running " + args.Message.Body + ", Credentials: " +
                           args.Principal.ToFriendlyString());
            };
            listener.ScopeClosing += (sender, args) =>
            {
                if (args.Exception != null)
                    return;

                var all = args.Scope.ResolveDependency<IAdoNetUnitOfWork>().ToList();
                all[0].SaveChanges();
            };
            listener.MessageInvokerFactory = MessageInvokerFactory;
            return listener;
        }

        private IMessageInvoker CreateMessageInvoker(IServiceProvider x)
        {
            var provider = (ServiceProvider) x;
            var invoker = new MessageInvoker(new HandlerScopeWrapper(provider));
            invoker.HandlerMissing += (sender, args) =>
            {
                _log.Error("No handler for " + args.Message.Body.GetType());
                try
                {
                    throw new NoHandlerRegisteredException(
                        "Failed to find a handler for " + args.Message.Body.GetType());
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {args.Message});
                }
            };

            invoker.InvokingHandler += (sender, args) =>
            {
                _log.Debug($"Invoking {JsonConvert.SerializeObject(args.Message)} ({args.Handler.GetType()}).");
            };
            invoker.HandlerInvoked += (sender, args) =>
            {
                _log.Debug($".. completed {args.Handler.GetType()}");
                if (args.Exception == null)
                    return;

                _log.Error(
                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED.",
                    args.Exception);

                Err.Report(args.Exception, new
                {
                    args.Message,
                    HandlerType = args.Handler.GetType(),
                    args.ExecutionTime
                });
            };
            return invoker;
        }

        private AdoNetMessageQueueProvider CreateQueueProvider(ConfigurationContext context)
        {
            if (_queueProvider != null)
                return _queueProvider;

            Func<IDbConnection> factory = () => context.ConnectionFactory(_systemPrincipal);
            var serializer = new MessagingSerializer(typeof(AdoNetMessageDto));
            //serializer.ThrowExceptionOnDeserialziationFailure = false;
            _queueProvider = new AdoNetMessageQueueProvider(factory, serializer);
            return _queueProvider;
        }

        private void DiagnosticLog(LogLevel level, string queuenameormessagename, string message)
        {
            _log.Info($"{queuenameormessagename}: {message}");
        }

        private IMessageInvoker MessageInvokerFactory(IHandlerScope arg)
        {
            var invoker = new MessageInvoker(arg);
            invoker.HandlerMissing += (sender, args) =>
            {
                try
                {
                    throw new NoHandlerRegisteredException(
                        "Failed to find a handler for " + args.Message.Body.GetType());
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {args.Message});
                }
            };
            invoker.HandlerInvoked += (sender, args) =>
            {
                if (args.Exception == null)
                    return;

                Err.Report(args.Exception, new
                {
                    args.Message,
                    HandlerType = args.Handler.GetType(),
                    args.ExecutionTime
                });
                _log.Error(
                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED.",
                    args.Exception);
            };
            return invoker;
        }
    }
}