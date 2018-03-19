using System;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Infrastructure.Boot
{
    public class ConfigurationContext
    {
        public Func<ClaimsPrincipal, IDbConnection> ConnectionFactory { get; set; }
        public IServiceCollection Services { get; set; }
        public Func<IServiceProvider> ServiceProvider { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}