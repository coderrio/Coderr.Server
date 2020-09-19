using System;
using System.Diagnostics;
using Coderr.Client;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Web.Boot.Adapters;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.Web.Boot.Modules
{
    public class BackgroundJobsForApps : IAppModule
    {
        private BackgroundJobManager _backgroundJobManager;


        public void Start(StartContext context)
        {
            var adapter = new DependencyInjectionAdapter(context.ServiceProvider);

            _backgroundJobManager = new BackgroundJobManager(adapter)
            {
                ExecuteSequentially = true,
                StartInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 10),
                ExecuteInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 30)
            };
            _backgroundJobManager.JobFailed += OnBackgroundJobFailed;
            _backgroundJobManager.ScopeClosing += OnBackgroundJobScopeClosing;
            _backgroundJobManager.Start();
        }

        public void Stop()
        {
            _backgroundJobManager?.Stop();
        }

        public void Configure(ConfigurationContext context)
        {
        }

        private void OnBackgroundJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
            Err.Report(e.Exception, new
            {
                JobType = e.Job.GetType().FullName
            });
        }

        private void OnBackgroundJobScopeClosing(object sender, ScopeClosingEventArgs e)
        {
            if (e.Successful)
                e.Scope.Resolve<IAdoNetUnitOfWork>().SaveChanges();
        }
    }
}