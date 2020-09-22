using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Abstractions.Feedback;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
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

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    internal class ReportQueueModule : IReportAnalyzerModule
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportQueueModule));
        private readonly ClaimsPrincipal _systemPrincipal;
        private QueueListener _eventProcessor;
        private IMessageQueueProvider _messageQueueProvider;
        private QueueListener _reportListener;

        public ReportQueueModule()
        {
            _systemPrincipal = new ClaimsPrincipal();
        }

        public void Configure(ConfigurationContext context)
        {
            ConfigureMessageQueueProvider(context);
            ConfigureListeners(context);
            ConfigureMessageHandlers(context);
            CreateDomainQueue(context);
        }

        public void Start(StartContext context)
        {
            _reportListener.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
            _eventProcessor.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void ConfigureListeners(ConfigurationContext context)
        {
            _reportListener = ConfigureQueueListener(context, "ErrorReports", "ErrorReportEvents");
            _eventProcessor = ConfigureQueueListener(context, "ErrorReportEvents", "ErrorReportEvents");
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

        private void ConfigureMessageQueueProvider(ConfigurationContext context)
        {
            var serializer = new MessagingSerializer(typeof(AdoNetMessageDto));
            _messageQueueProvider =
                new AdoNetMessageQueueProvider(() => context.ConnectionFactory(_systemPrincipal), serializer);
        }

        private QueueListener ConfigureQueueListener(ConfigurationContext context, string inboundQueueName,
            string outboundQueueName)
        {
            var inboundQueue = _messageQueueProvider.Open(inboundQueueName);
            var outboundQueue = inboundQueueName == outboundQueueName
                ? inboundQueue
                : _messageQueueProvider.Open(outboundQueueName);
            var scopeFactory = new ScopeWrapper(context.ServiceProvider);
            var router = new MessageRouter
            {
                ReportAnalyzerQueue = outboundQueue, AppQueue = _messageQueueProvider.Open("Messaging")
            };

            var listener = new QueueListener(inboundQueue, router, scopeFactory)
            {
                RetryAttempts =
                    new[] {TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},
                MessageInvokerFactory = scope =>
                {
                    var invoker = new MessageInvoker(scope);
                    invoker.Logger += (level, name, message) => _logger.Debug($"[{name}] {message}");
                    invoker.InvokingHandler += (sender, args) =>
                    {
                        _logger.Debug(
                            $"[{inboundQueue.Name}] Invoking {JsonConvert.SerializeObject(args.Message)} ({args.Handler.GetType()}).");
                    };
                    return invoker;
                },
                Logger = DiagnosticLog
            };
            listener.PoisonMessageDetected += (sender, args) =>
            {
                Err.Report(args.Exception, new {args.Message.Body});
                _logger.Error($"[{inboundQueueName}] Poison message: {args.Message.Body}", args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                args.Scope.ResolveDependency<IPrincipalAccessor>().First().Principal = args.Principal;
            };
            listener.ScopeClosing += (sender, args) =>
            {
                if (args.Exception != null)
                    return;

                var all = args.Scope.ResolveDependency<IAdoNetUnitOfWork>().ToList();
                all[0].SaveChanges();

                var queue = (DomainQueueWrapper) args.Scope.ResolveDependency<IDomainQueue>().First();
                queue.SaveChanges();
            };
            listener.MessageInvokerFactory = MessageInvokerFactory;
            return listener;
        }

        /// <summary>
        ///     Writes to the message queue that the application is processing (publishing in the other bounded context)
        /// </summary>
        /// <param name="context"></param>
        private void CreateDomainQueue(ConfigurationContext context)
        {
            context.Services.AddScoped<IDomainQueue>(x =>
            {
                var queue = _messageQueueProvider.Open("Messaging");
                var messageBus = new ScopedMessageBus(queue);
                return new DomainQueueWrapper(messageBus);
            });
        }


        private void DiagnosticLog(LogLevel level, string queueNameOrMessageName, string message)
        {
            _logger.Debug($"[{queueNameOrMessageName}] {message}");
        }

        private void HaveRun(Task obj)
        {
            _logger.Info("Stop completed for a listener in the ReportQueueModule. " + obj);
        }

        private IMessageInvoker MessageInvokerFactory(IHandlerScope arg)
        {
            var k = arg.ResolveDependency<IMessageHandler<FeedbackAttachedToIncident>>();
            var invoker = new MessageInvoker(arg);
            invoker.HandlerMissing += (sender, args) =>
            {
                _logger.Warn(
                    "Failed to find a handler for " + args.Message.Body.GetType());
            };
            invoker.HandlerInvoked += (sender, args) =>
            {
                if (args.Exception == null)
                    return;

                Err.Report(args.Exception,
                    new {args.Message, HandlerType = args.Handler.GetType(), args.ExecutionTime});
                _logger.Error(
                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED.",
                    args.Exception);
            };
            return invoker;
        }
    }
}