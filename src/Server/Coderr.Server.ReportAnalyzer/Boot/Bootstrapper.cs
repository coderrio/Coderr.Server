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
        private readonly ReportAnalyzerModuleStarter _reportAnalyzerReportAnalyzerModuleStarter;
        private ServiceProvider _serviceProvider;

        public Bootstrapper()
        {
            _reportAnalyzerReportAnalyzerModuleStarter = new ReportAnalyzerModuleStarter();
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
            _reportAnalyzerReportAnalyzerModuleStarter.Configure(ourContext, new ServerConfigSectionWrapper(ignoredModules));

            RegisterConfigurationStores(ourContext);
            ourContext.Services.AddScoped<IPrincipalAccessor, MessagingPrincipalWrapper>();
            ourContext.Services.AddScoped<IAdoNetUnitOfWork>(x =>
                new AnalysisUnitOfWork(x.GetService<IDbConnection>(), false));
            ourContext.Services.AddScoped(provider =>
            {
                var principal = provider.GetService<IPrincipalAccessor>().Principal 
                                ?? CoderrClaims.SystemPrincipal;

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
            _reportAnalyzerReportAnalyzerModuleStarter.Start(ourStartContext);
        }


        public void Stop()
        {
            _reportAnalyzerReportAnalyzerModuleStarter.Stop();
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
                new DatabaseStore(() => context.ConnectionFactory(CoderrClaims.SystemPrincipal)));
        }
    }
}