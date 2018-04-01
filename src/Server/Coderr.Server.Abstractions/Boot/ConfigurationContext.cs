using System;
using System.Data;
using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Abstractions.Boot
{
    public abstract class ConfigurationContext
    {

        public Func<ClaimsPrincipal, IDbConnection> ConnectionFactory { get; set; }
        public IServiceCollection Services { get; set; }
        public Func<IServiceProvider> ServiceProvider { get; set; }
        public IConfiguration Configuration { get; set; }

        public abstract void RegisterMessageTypes(Assembly assembly);
    }
}