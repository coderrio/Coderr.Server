using System;
using System.Diagnostics;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;
using OneTrueError.Web.Cqs;

namespace OneTrueError.Web.Services
{
    public class ServiceRunner : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (ServiceRunner));
        private ApplicationServiceManager _appManager;
        private BackgroundJobManager _backgroundJobManager;
        private readonly CompositionRoot _compositionRoot = new CompositionRoot();
        private readonly CqsBuilder _cqsBuilder = new CqsBuilder();

        public IContainer Container
        {
            get { return CompositionRoot.Container; }
        }

        public void Start()
        {
            _log.Debug("Starting ...");
            try
            {
                _compositionRoot.Build(registrar =>
                {
                    registrar.RegisterService(x => _cqsBuilder.CreateCommandBus(Container), Lifetime.Singleton);
                    registrar.RegisterService(x => _cqsBuilder.CreateQueryBus(Container), Lifetime.Singleton);
                    registrar.RegisterService(x => _cqsBuilder.CreateEventBus(Container), Lifetime.Singleton);
                    registrar.RegisterService(x => _cqsBuilder.CreateRequestReplyBus(Container), Lifetime.Singleton);
                });
                BuildServices();
                ConfigureDataMapper();

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

        private static void ConfigureDataMapper()
        {
            var provider = new AssemblyScanningMappingProvider();
            provider.Scan();
            EntityMappingProvider.Provider = provider;
        }


        private void BuildServices()
        {
            _appManager = new ApplicationServiceManager(CompositionRoot.Container);
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

        private void OnServiceFailed(object sender, ApplicationServiceFailedEventArgs e)
        {
            _log.Error("Failed to execute " + e.ApplicationService, e.Exception);
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

        public void Stop()
        {
            _backgroundJobManager.Stop();
            _appManager.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_backgroundJobManager != null)
            {
                _backgroundJobManager.Dispose();
                _backgroundJobManager = null;
            }
        }
    }
}