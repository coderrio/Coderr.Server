using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Connections.Queries;
using Coderr.Server.Common.AzureDevOps.App.WorkItems;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Plugins.AzureDevOps.App
{
    public class Boot : IAppModule
    {
        public void Configure(ConfigurationContext context)
        {
            var assembly = typeof(GetAreaPathsHandler).Assembly;
            context.Services.RegisterContainerServices(assembly);
            context.Services.RegisterMessageHandlers(assembly);
            context.RegisterMessageTypes(typeof(GetProjects).Assembly);
        }

        public void Start(StartContext context)
        {
            context.ServiceProvider.GetService<IWorkItemServiceRegistrar>().Register<AzureDevOpsWorkItemService>(AzureDevOpsWorkItemService.NAME);
        }

        public void Stop()
        {
            
        }
    }
}
