using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot
{
    public class Bootstrapper : IAppModule
    {
        private readonly ModuleStarter _moduleStarter;
        private ServiceProvider _serviceProvider;
        private readonly ClaimsPrincipal _systemPrincipal;

        public Bootstrapper()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Id"),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, CoderrRoles.System)
            };
            var identity = new ClaimsIdentity(claims);
            _systemPrincipal = new ClaimsPrincipal(identity);
            _moduleStarter = new ModuleStarter();
        }

        public void Configure(ConfigurationContext context)
        {
            var serviceCollection = new ServiceCollection();

            var ourContext = new Abstractions.Boot.ConfigurationContext
            {
                Configuration = new ServerConfigWrapper(context.Configuration),
                ConnectionFactory = context.ConnectionFactory,
                ServiceProvider = GetServiceProvider,
                Services = serviceCollection
            };
            var ignoredModules = context.Configuration.GetSection("DisabledModules:ReportAnalyzer");
            _moduleStarter.Configure(ourContext, new ServerConfigSectionWrapper(ignoredModules));

            RegisterConfigurationStores(ourContext);
            ourContext.Services.AddScoped<IPrincipalAccessor, MessagingPrincipalWrapper>();
            ourContext.Services.AddScoped<IAdoNetUnitOfWork>(x =>
                new AnalysisUnitOfWork(x.GetService<IDbConnection>(), false));
            ourContext.Services.AddScoped(provider =>
            {
                var principal = provider.GetService<IPrincipalAccessor>().Principal;
                if (principal == null)
                    principal = _systemPrincipal;
                return context.ConnectionFactory(principal);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }


        public void Start(StartContext context)
        {
            var ourStartContext = new Abstractions.Boot.StartContext
            {
                ServiceProvider = _serviceProvider
            };
            _moduleStarter.Start(ourStartContext);
        }


        public void Stop()
        {
            _moduleStarter.Stop();
        }

        private IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("service provider have not been built yet.");
            return _serviceProvider;
        }

        private void RegisterConfigurationStores(Abstractions.Boot.ConfigurationContext context)
        {
            context.Services.AddScoped(typeof(IConfiguration<>), typeof(ConfigWrapper<>));
            context.Services.AddScoped<ConfigurationStore>(x =>
                new DatabaseStore(() => context.ConnectionFactory(_systemPrincipal)));
        }
    }
}