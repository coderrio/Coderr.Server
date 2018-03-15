using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using codeRR.Client;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Messaging;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.Web.Cqs;
using codeRR.Server.Web.Infrastructure;
using codeRR.Server.Web.IoC;
using DotNetCqs;
using DotNetCqs.Bus;
using DotNetCqs.DependencyInjection;
using DotNetCqs.MessageProcessor;
using DotNetCqs.Queues;
using DotNetCqs.Queues.AdoNet;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;
using Owin;
using WebGrease.Css.Extensions;
using ScopeClosingEventArgs = Griffin.ApplicationServices.ScopeClosingEventArgs;

namespace codeRR.Server.Web.Services
{
    /// <summary>
    ///     Used to configure the back-end. It's a mess, but a limited mess.
    /// </summary>
    public class ServiceRunner : IDisposable
    {
        private readonly CompositionRoot _compositionRoot = new CompositionRoot();
        private readonly ILog _log = LogManager.GetLogger(typeof(ServiceRunner));
        private readonly PluginManager _pluginManager = new PluginManager();
        private readonly CancellationTokenSource _shutdownToken = new CancellationTokenSource();
        private ApplicationServiceManager _appManager;
        private BackgroundJobManager _backgroundJobManager;
        private PluginConfiguration _pluginConfiguration;
        private AdoNetMessageQueueProvider _queueProvider;

        public ServiceRunner()
        {
            _queueProvider = CreateQueueProvider();
        }

        public IContainer Container => CompositionRoot.Container;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start(IAppBuilder app)
        {
            WarmupPlugins();


            _log.Debug("Starting ...");
            try
            {
                _compositionRoot.Build(registrar =>
                {
                    LetPluginsRegisterServices(registrar);

                    registrar.RegisterService<IMessageBus>(x =>
                    {
                        var queue = x.Resolve<IMessageQueueProvider>().Open("Messaging");
                        var bus = new SingleInstanceMessageBus(queue);
                        return bus;
                    }, Lifetime.Singleton);
                    registrar.RegisterConcrete<ScopedQueryBus>();
                    registrar.RegisterService(CreateMessageInvoker, Lifetime.Scoped);

                    registrar.RegisterService(x => CreateQueueListener(x, "Messaging", "Messaging"), Lifetime.Singleton);
                    registrar.RegisterService(x => CreateQueueListener(x, "Reports", "Messaging"), Lifetime.Singleton);
                    registrar.RegisterService(x => CreateQueueListener(x, "Feedback", "Messaging"), Lifetime.Singleton);
                    registrar.RegisterInstance<IMessageQueueProvider>(_queueProvider);

                    // let us guard it since it runs events in the background.
                    //var service = registrar.Registrations.First(x => x.Implements(typeof(IMessageBus)));
                    //service.AddService(typeof(IApplicationService));
                }, Startup.ConfigurationStore);

                var listeners = Container.ResolveAll<QueueListener>().ToArray();
                listeners
                    .ForEach(x => x
                        .RunAsync(_shutdownToken.Token)
                        .ContinueWith(OnStopped)
                    );

                BuildServices();
                _appManager.Start();
                _backgroundJobManager.Start();
                _log.Debug("...started");
            }
            catch (Exception exception)
            {
                _log.Error("Failed to start.", exception);
                throw;
            }
        }

        public void Stop()
        {
            _backgroundJobManager.Stop();
            _appManager.Stop();
            _shutdownToken.Cancel();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_backgroundJobManager != null)
            {
                _backgroundJobManager.Dispose();
                _backgroundJobManager = null;
            }
        }

        private void BuildServices()
        {
            _appManager = new ApplicationServiceManager(CompositionRoot.Container)
            {
                Settings = new ApplicationServiceManagerSettingsWithDefaultOn()
            };
            _appManager.ServiceFailed += OnServiceFailed;

            _backgroundJobManager = new BackgroundJobManager(CompositionRoot.Container);
            _backgroundJobManager.JobFailed += OnJobFailed;
            _backgroundJobManager.StartInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 10);
            _backgroundJobManager.ExecuteInterval = TimeSpan.FromSeconds(30);

            _backgroundJobManager.ScopeClosing += OnBackgroundJobScopeClosing;
        }

        private IMessageInvoker CreateMessageInvoker(IServiceLocator x)
        {
            var scope = new GriffinContainerScopeAdapter((IChildContainer)x);
            var invoker = new MessageInvoker(scope);
            invoker.HandlerMissing += (sender, args) =>
            {
                try
                {
                    throw new NoHandlerRegisteredException(
                        "Failed to find a handler for " + args.Message.Body.GetType());
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new { args.Message });
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

        private QueueListener CreateQueueListener(IServiceLocator locator, string inboundQueueName,
            string outboundQueueName)
        {
            var provider = locator.Resolve<IMessageQueueProvider>();
            var inboundQueue = provider.Open(inboundQueueName);
            var outboundQueue = inboundQueueName == outboundQueueName ? inboundQueue : provider.Open(outboundQueueName);
            var listener = new QueueListener(inboundQueue, outboundQueue, new GriffinHandlerScopeFactory(Container))
            {
                RetryAttempts = new[]
                    {TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},
                MessageInvokerFactory = scope => new MessageInvoker(scope),
                Logger = Logga
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
            return listener;
        }

        private AdoNetMessageQueueProvider CreateQueueProvider()
        {
            Func<IDbConnection> dbFactory = () => DbConnectionFactory.Open(true);
            var serializer = new MessagingSerializer(typeof(AdoNetMessageDto));
            var provider = new AdoNetMessageQueueProvider(dbFactory, serializer);
            return provider;
        }

        private void LetPluginsRegisterServices(ContainerRegistrar registrar)
        {
            _pluginConfiguration = new PluginConfiguration(registrar);
            _pluginManager.ConfigurePlugins(_pluginConfiguration);
        }

        private void Logga(LogLevel level, string queuenameormessagename, string message)
        {
        }


        private void OnBackgroundJobScopeClosing(object sender, ScopeClosingEventArgs e)
        {
            try
            {
                if (e.Successful)
                    e.Scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
            }
            catch (Exception exception)
            {
                Err.Report(exception);
                _log.Error("Failed to close scope.", exception);
            }
        }


        private void OnJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.Job, e.Exception);
            try
            {
                Err.Report(e.Exception, new { JobType = e.Job?.GetType().FullName });
            }
            catch (Exception ex)
            {
                _log.Error("Failed to report.", ex);
            }

        }

        private void OnServiceFailed(object sender, ApplicationServiceFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.ApplicationService, e.Exception);
            try
            {
                Err.Report(e.Exception, new { JobType = e.ApplicationService?.GetType().FullName });
            }
            catch (Exception exception)
            {
                _log.Error("Failed to report.", exception);
            }
        }

        private void OnStopped(Task obj)
        {
            _log.Warn("QueueListener stopped");
        }

        private void WarmupPlugins()
        {
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            _log.Debug("Loading from " + pluginPath);
            _pluginManager.Load(pluginPath);
            _pluginManager.PreloadPlugins();
        }
    }
}