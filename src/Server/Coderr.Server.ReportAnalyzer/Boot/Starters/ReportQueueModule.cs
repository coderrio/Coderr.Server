using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using codeRR.Client;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.Infrastructure.Security;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using DotNetCqs;
using DotNetCqs.DependencyInjection;
using DotNetCqs.MessageProcessor;
using DotNetCqs.Queues;
using DotNetCqs.Queues.AdoNet;
using Griffin.Data;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    internal class ReportQueueModule
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportQueueModule));
        private readonly ClaimsPrincipal _systemPrincipal;
        private QueueListener _eventProcessor;
        private IMessageQueueProvider _messageQueueProvider;
        private QueueListener _reportListener;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ReportQueueModule()
        {
            _systemPrincipal = new ClaimsPrincipal();
        }

        public void Configure(ConfigurationContext context)
        {
            ConfigureMessageQueueProvider(context);
            ConfigureListeners(context);
            ConfigureMessageHandlers(context);
        }

        private void ConfigureListeners(ConfigurationContext context)
        {
            _reportListener = ConfigureQueueListener(context, "ErrorReports", "ErrorReportEvents");
            _eventProcessor = ConfigureQueueListener(context, "ErrorReportEvents", "ErrorReportEvents");
        }

        private void ConfigureMessageHandlers(ConfigurationContext context)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(y => y.GetInterfaces().Any(x => x.Name.Contains("IMessageHandler")))
                .ToList();
            foreach (var type in types)
            {
                context.Services.AddScoped(type, type);
                context.Services.AddScoped(type.GetInterfaces()[0], type);
            }
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
                _logger.Error(inboundQueueName + " Poison message: " + args.Message.Body, args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                args.Scope.ResolveDependency<ScopedPrincipal>().First().Principal = args.Principal;
                
                _logger.Debug(inboundQueueName + " Running " + args.Message.Body + ", Credentials: " +
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


        private void DiagnosticLog(LogLevel level, string queueNameOrMessageName, string message)
        {
            
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
                _logger.Error(
                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED.",
                    args.Exception);
            };
            return invoker;
        }

        public void Start(StartContext context)
        {
            _reportListener.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
            _eventProcessor.RunAsync(_cancellationTokenSource.Token).ContinueWith(HaveRun);
        }

        private void HaveRun(Task obj)
        {
            
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}