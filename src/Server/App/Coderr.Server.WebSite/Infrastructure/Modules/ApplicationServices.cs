using Coderr.Server.Abstractions.Boot;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using Griffin.ApplicationServices;

namespace Coderr.Server.WebSite.Infrastructure.Modules
{
    public class ApplicationServices : IAppModule
    {
        private ApplicationServiceManager _appManager;
        private IConfigurationSection _configuration;

        public void Start(StartContext context)
        {
            var adapter = new DependencyInjectionAdapter(context.ServiceProvider);
            _appManager = _appManager = new ApplicationServiceManager(adapter)
            {
                Settings = new ApplicationServiceManagerSettingsWithDefaultOn(_configuration)
            };
            _appManager.ServiceFailed += OnServiceFailed;
            _appManager.Start();
        }

        public void Stop()
        {
            _appManager?.Stop();
        }

        public void Configure(ConfigurationContext context)
        {
            _configuration = context.Configuration.GetSection("ApplicationServices");
        }

        private void OnServiceFailed(object sender, ApplicationServiceFailedEventArgs e)
        {
        }
    }
}