using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Security;
using Coderr.Server.PluginApi.Config;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using Coderr.Server.ReportAnalyzer.Boot.Starters;
using Coderr.Server.Web.Boot.Adapters;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot
{
    public class Bootstrapper : ISystemModule
    {
        private readonly ReportQueueModule _queueModule = new ReportQueueModule();
        private readonly RegisterContainerServices _registerContainerServices = new RegisterContainerServices();
        private ServiceProvider _serviceProvider;
        private ClaimsPrincipal _systemPrincipal;

        public Bootstrapper()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Id"),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, CoderrClaims.RoleSystem)
            };
            var identity = new ClaimsIdentity(claims);
            _systemPrincipal = new ClaimsPrincipal(identity);
        }

        public void Configure(ConfigurationContext context)
        {

            var serviceCollection = new ServiceCollection();

            var ourContext = new ConfigurationContext
            {
                Configuration = context.Configuration,
                ConnectionFactory = context.ConnectionFactory,
                ServiceProvider = GetServiceProvider,
                Services = serviceCollection
            };
            _queueModule.Configure(ourContext);
            _registerContainerServices.Configure(ourContext);

            RegisterConfigurationStores(ourContext);
            ourContext.Services.AddScoped<ScopedPrincipal>();
            ourContext.Services.AddScoped<IAdoNetUnitOfWork>(x => new AnalysisUnitOfWork(x.GetService<IDbConnection>(), false));
            ourContext.Services.AddScoped(provider =>
            {
                var principal = provider.GetService<ScopedPrincipal>().Principal;
                if (principal == null)
                    principal = _systemPrincipal;
                return context.ConnectionFactory(principal);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void RegisterConfigurationStores(ConfigurationContext context)
        {
            context.Services.AddScoped(typeof(IConfiguration<>), typeof(ConfigWrapper<>));
            context.Services.AddScoped<ConfigurationStore>(x => new DatabaseStore(() => context.ConnectionFactory(_systemPrincipal)));
        }


        public void Start(StartContext context)
        {
            var ourStartContext = new StartContext
            {
                ServiceProvider = _serviceProvider
            };
            _queueModule.Start(ourStartContext);
        }

        public void Stop()
        {
            _queueModule.Stop();
        }

        private IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("service provider have not been built yet.");
            return _serviceProvider;
        }
    }
}