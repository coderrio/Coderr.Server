using System;
using System.Diagnostics;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Web2.Boot.Adapters;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.Web2.Boot.Modules
{
    public class BackgroundServices : ISystemModule
    {
        private ApplicationServiceManager _appManager;
        private BackgroundJobManager _backgroundJobManager;
        private IConfiguration _configuration;


        public void Start(StartContext context)
        {
            var adapter = new DependencyInjectionAdapter(context.ServiceProvider);
            _appManager = _appManager = new ApplicationServiceManager(adapter)
            {
                Settings = new ApplicationServiceManagerSettingsWithDefaultOn(_configuration)
            };
            _appManager.ServiceFailed += OnServiceFailed;
            _appManager.Start();

            _backgroundJobManager = new BackgroundJobManager(adapter);
            _backgroundJobManager.JobFailed += OnBackgroundJobFailed;
            _backgroundJobManager.StartInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 10);
            _backgroundJobManager.ExecuteInterval = TimeSpan.FromMinutes(3);
            _backgroundJobManager.ScopeClosing += OnBackgroundJobScopeClosing;
            _backgroundJobManager.Start();
        }

        public void Stop()
        {
            _backgroundJobManager?.Stop();
            _appManager?.Stop();
        }

        public void Configure(ConfigurationContext context)
        {
            _configuration = context.Configuration;
        }

        private void OnBackgroundJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
        }

        private void OnBackgroundJobScopeClosing(object sender, ScopeClosingEventArgs e)
        {
            if (e.Successful)
                e.Scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
        }

        private void OnServiceFailed(object sender, ApplicationServiceFailedEventArgs e)
        {
        }
    }
}