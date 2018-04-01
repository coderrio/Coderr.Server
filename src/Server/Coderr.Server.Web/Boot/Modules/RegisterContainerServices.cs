using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.SqlServer;

namespace Coderr.Server.Web.Boot.Modules
{
    public class RegisterContainerServices : IAppModule
    {
        public void Configure(ConfigurationContext context)
        {
            var assembly = typeof(IAccountService).Assembly;
            context.Services.RegisterContainerServices(assembly);

            assembly = typeof(SqlServerTools).Assembly;
            context.Services.RegisterContainerServices(assembly);
        }

        public void Start(StartContext context)
        {
        }

        public void Stop()
        {
        }
    }
}