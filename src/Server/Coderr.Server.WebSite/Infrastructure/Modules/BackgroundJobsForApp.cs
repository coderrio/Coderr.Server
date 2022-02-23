using System;
using System.Diagnostics;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using Griffin.ApplicationServices;
using Griffin.Data;
using log4net;

namespace Coderr.Server.WebSite.Infrastructure.Modules
{
    public class BackgroundJobsForApp : IAppModule
    {
        private BackgroundJobManager _backgroundJobManager;
        private IConfiguration _configuration;
        private ILog _logger = LogManager.GetLogger(typeof(BackgroundJobManager));

        public void Start(StartContext context)
        {
            var adapter = new DependencyInjectionAdapter(context.ServiceProvider);

            _backgroundJobManager = new BackgroundJobManager(adapter)
            {
                ExecuteSequentially = true,
                StartInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 10),
                ExecuteInterval = TimeSpan.FromMinutes(3)
            };
            _backgroundJobManager.JobFailed += OnBackgroundJobFailed;
            _backgroundJobManager.StartInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 0 : 10);
            _backgroundJobManager.ExecuteInterval = TimeSpan.FromSeconds(Debugger.IsAttached ? 10 : 30);
            _backgroundJobManager.ScopeClosing += OnBackgroundJobScopeClosing;
            _backgroundJobManager.Start();
        }


        public void Stop()
        {
            _backgroundJobManager?.Stop();
        }

        public void Configure(ConfigurationContext context)
        {
            _configuration = context.Configuration;
        }

        private void OnBackgroundJobFailed(object sender, BackgroundJobFailedEventArgs e)
        {
            _logger.Error("Job failed: " + e.Job, e.Exception);
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