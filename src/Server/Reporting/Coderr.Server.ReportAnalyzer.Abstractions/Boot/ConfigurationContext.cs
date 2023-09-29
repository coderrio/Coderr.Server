using System;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public class ConfigurationContext
    {
        public ConfigurationContext(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory)
        {
            Services= serviceCollection;
            ServiceProvider = serviceProviderFactory;
        }

        public Func<ClaimsPrincipal, IDbConnection> ConnectionFactory { get; set; }
        
        public IServiceCollection Services { get; private set; }

        public Func<IServiceProvider> ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; set; }
    }
}