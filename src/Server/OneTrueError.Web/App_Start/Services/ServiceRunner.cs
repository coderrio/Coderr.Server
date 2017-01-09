using System;
using System.Diagnostics;
using System.Linq;
using DotNetCqs;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;
using OneTrueError.Web.Cqs;
using Owin;

namespace OneTrueError.Web.Services
{
    /// <summary>
    ///     Used to configure the back-end. It's a mess, but a limited mess.
    /// </summary>
    public class ServiceRunner : IDisposable
    {
        private readonly CompositionRoot _compositionRoot = new CompositionRoot();
        private readonly CqsBuilder _cqsBuilder = new CqsBuilder();
        private readonly ILog _log = LogManager.GetLogger(typeof(ServiceRunner));
        private ApplicationServiceManager _appManager;
        private BackgroundJobManager _backgroundJobManager;

        public IContainer Container
        {
            get { return CompositionRoot.Container; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start(IAppBuilder app)
        {
            _log.Debug("Starting ...");
            try
            {
                _compositionRoot.Build(registrar =>
                {
                    registrar.RegisterService(x => _cqsBuilder.CreateCommandBus(Container), Lifetime.Singleton);
                    registrar.RegisterService(x => _cqsBuilder.CreateQueryBus(Container), Lifetime.Singleton);
                    registrar.RegisterService(x => _cqsBuilder.CreateEventBus(Container), Lifetime.Singleton);

                    // let us guard it since it runs events in the background.
                    var service = registrar.Registrations.First(x => x.Implements(typeof(IEventBus)));
                    service.AddService(typeof(IApplicationService));

                    registrar.RegisterService(x => _cqsBuilder.CreateRequestReplyBus(Container), Lifetime.Singleton);
                });

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
            _appManager = new ApplicationServiceManager(CompositionRoot.Container);
            _appManager.Settings = new ApplicationServiceManagerSettingsWithDefaultOn();
            _appManager.ServiceFailed += OnServiceFailed;

            _backgroundJobManager = new BackgroundJobManager(CompositionRoot.Container);
            _backgroundJobManager.JobFailed += OnJobFailed;
            if (Debugger.IsAttached)
                _backgroundJobManager.StartInterval = TimeSpan.FromSeconds(0);
            else
                _backgroundJobManager.StartInterval = TimeSpan.FromSeconds(10);
            _backgroundJobManager.ExecuteInterval = TimeSpan.FromMinutes(5);

            _backgroundJobManager.ScopeClosing += OnScopeClosing;
        }


        private void OnJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.Job, e.Exception);
        }


        private void OnScopeClosing(object sender, ScopeClosingEventArgs e)
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
    }
}