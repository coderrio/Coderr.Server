using System;
using System.Data;
using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Abstractions.Boot
{
    public abstract class ConfigurationContext
    {
        protected ConfigurationContext(IServiceCollection services, Func<IServiceProvider> serviceProvider)
        {
            Services = services;
            ServiceProvider = serviceProvider;
        }

        public Func<ClaimsPrincipal, IDbConnection> ConnectionFactory { get; set; }
        public IServiceCollection Services { get; private set; }
        public Func<IServiceProvider> ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; set; }

        public abstract void RegisterMessageTypes(Assembly assembly);

        public abstract ConfigurationContext Clone(IServiceCollection serviceCollection);
    }
}