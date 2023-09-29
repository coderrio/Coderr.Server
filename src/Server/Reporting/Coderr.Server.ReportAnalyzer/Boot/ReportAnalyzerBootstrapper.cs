using System;
using System.Data;
using System.Linq;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Reports;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot
{
    public class ReportAnalyzerBootstrapper : IAppModule
    {
        private readonly ReportAnalyzerModuleStarter _reportAnalyzerModuleStarter;
        private ServiceProvider _serviceProvider;

        public ReportAnalyzerBootstrapper()
        {
            _reportAnalyzerModuleStarter = new ReportAnalyzerModuleStarter();
        }

        public void Configure(ConfigurationContext context)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(context.Configuration);

            var ourContext = new Abstractions.Boot.ConfigurationContext(serviceCollection, GetServiceProvider)
            {
                Configuration = new ServerConfigWrapper(context.Configuration),
                ConnectionFactory = context.ConnectionFactory,
            };
            var ignoredModules = context.Configuration.GetSection("DisabledModules:ReportAnalyzer");
            _reportAnalyzerModuleStarter.Configure(ourContext, new ServerConfigSectionWrapper(ignoredModules));

            if (!ServerConfig.Instance.IsLive)
            {
                ourContext.Services.AddScoped(typeof(IConfiguration<>), typeof(ConfigWrapper<>));
                ourContext.Services.AddSingleton<ConfigurationStore>(x =>
                    new DatabaseStore(() => context.ConnectionFactory(CoderrClaims.SystemPrincipal)));
            }

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
            var collection = context.Services.Where(x => x.ServiceType.Name.Contains("IConfiguration")).ToList();
            var b = _serviceProvider.GetService<ConfigurationStore>();
            var d = _serviceProvider.GetRequiredService(typeof(IConfiguration<>).MakeGenericType(typeof(ReportConfig)));
        }


        public void Start(StartContext context)
        {
            var ourStartContext = new Abstractions.Boot.StartContext
            {
                ServiceProvider = _serviceProvider
            };
            _reportAnalyzerModuleStarter.Start(ourStartContext);
        }


        public void Stop()
        {
            _reportAnalyzerModuleStarter.Stop();
        }

        private IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("service provider have not been built yet.");
            return _serviceProvider;
        }
    }
}