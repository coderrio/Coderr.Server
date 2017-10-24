using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Security;
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
using ScopeClosingEventArgs = Griffin.ApplicationServices.ScopeClosingEventArgs;

namespace codeRR.Server.Web.Services
{
    /// <summary>
    ///     Used to configure the back-end. It's a mess, but a limited mess.
    /// </summary>
    public class ServiceRunner : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly CompositionRoot _compositionRoot = new CompositionRoot();
        private readonly ILog _log = LogManager.GetLogger(typeof(ServiceRunner));
        private readonly PluginManager _pluginManager = new PluginManager();
        private ApplicationServiceManager _appManager;
        private BackgroundJobManager _backgroundJobManager;
        private PluginConfiguration _pluginConfiguration;
        private CancellationTokenSource _shutdownToken = new CancellationTokenSource();

        public ServiceRunner(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
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

                    registrar.RegisterConcrete<DotNetCqs.Bus.SingleInstanceMessageBus>(Lifetime.Singleton);
                    registrar.RegisterConcrete<ScopedQueryBus>();
                    registrar.RegisterInstance<IMessageQueue>(
                        new AdoNetMessageQueue("Messaging", () => _connectionFactory.Open(),
                            new JsonMessageQueueSerializer())
                        {
                            TableName = "MessageQueue"
                        });

                    registrar.RegisterService<IMessageInvoker>(x =>
                    {
                        var scope = new GriffinContainerScopeAdapter((IChildContainer)x);
                        var invoker = new MessageInvoker(scope);
                        invoker.InvokingHandler += (sender, args) =>
                        {
                            _log.Debug($"Invoking {args.Handler} in scope " + scope.GetHashCode());
                        };
                        invoker.HandlerInvoked += (sender, args) =>
                        {
                            if (args.Exception == null)
                            {
                                _log.Debug($"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms");
                            }
                            else
                            {
                                _log.Error(
                                    $"Ran {args.Handler}, took {args.ExecutionTime.TotalMilliseconds}ms, but FAILED.",
                                    args.Exception);
                            }
                        };
                        return invoker;
                    }, Lifetime.Scoped);
                    registrar.RegisterService(CreateQueueListener, Lifetime.Singleton);

                    // let us guard it since it runs events in the background.
                    //var service = registrar.Registrations.First(x => x.Implements(typeof(IMessageBus)));
                    //service.AddService(typeof(IApplicationService));

                }, Startup.ConfigurationStore);

                var listener = CreateQueueListener(null);
                listener.RunAsync(_shutdownToken.Token).ContinueWith(OnStopped);
                
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

        private void OnStopped(Task obj)
        {
            _log.Warn("QueueListener stopped");
        }

        private QueueListener CreateQueueListener(IServiceLocator serviceLocator)
        {
            var queue = Container.Resolve<IMessageQueue>();
            var listener = new QueueListener(queue, new GriffinHandlerScopeFactory(Container))
            {
                RetryAttempts = new[]
                    {TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},

            };
            listener.Logger = s => _log.Info(s);
            listener.PoisonMessageDetected += (sender, args) =>
            {
                _log.Error("Poison message: " + args.Message.Body, args.Exception);
            };
            listener.ScopeCreated += (sender, args) =>
            {
                _log.Debug("Running " + args.Message.Body + ", Credentials: " + args.Principal.ToFriendlyString());
            };
            listener.ScopeClosing += (sender, args) =>
            {
                if (args.Exception == null)
                {
                    var all = args.Scope.ResolveDependency<IAdoNetUnitOfWork>().ToList();
                    _log.Info("Saving " + all[0].GetHashCode() + " of " + all.Count + " from scope " + args.Scope.GetHashCode());
                    all[0].SaveChanges();
                }
                if (args.Exception != null)
                    _log.Error("Failed to process " + args.Message.Body + ".", args.Exception);
            };
            return listener;
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
            _backgroundJobManager.ExecuteInterval = TimeSpan.FromMinutes(5);

            _backgroundJobManager.ScopeClosing += OnBackgroundJobScopeClosing;
        }

        private void LetPluginsRegisterServices(ContainerRegistrar registrar)
        {
            _pluginConfiguration = new PluginConfiguration(registrar);
            _pluginManager.ConfigurePlugins(_pluginConfiguration);
        }


        private void OnJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.Job, e.Exception);
        }


        private void OnBackgroundJobScopeClosing(object sender, ScopeClosingEventArgs e)
        {
            try
            {
                e.Scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
            }
            catch (Exception exception)
            {
                _log.Error("Failed to close scope. Err: " + exception, e.Exception);
            }
        }

        private void OnServiceFailed(object sender, ApplicationServiceFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.ApplicationService, e.Exception);
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