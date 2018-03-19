using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.SqlServer;
using Griffin.Container;
using Microsoft.Extensions.DependencyInjection;
using ContainerServiceAttribute = Coderr.Server.ReportAnalyzer.Abstractions.ContainerServiceAttribute;

namespace Coderr.Server.Web2.Boot.Modules
{
    public class RegisterContainerServices : ISystemModule
    {
        public void Configure(ConfigurationContext context)
        {
            var assembly = typeof(IAccountService).Assembly;
            RegisterAssemblyServices(assembly, context);

            assembly = typeof(SqlServerTools).Assembly;
            RegisterAssemblyServices(assembly, context);
        }

        private static void RegisterAssemblyServices(Assembly assembly, ConfigurationContext context)
        {
            var containerServices = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<ContainerServiceAttribute>(true) != null
                || x.GetCustomAttribute<Griffin.Container.ContainerServiceAttribute>(true) != null)
                .ToList();
            foreach (var containerService in containerServices)
            {
                var interfaces = containerService.GetInterfaces();
                if (interfaces.Length != 1)
                    Debugger.Break();

                var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                if (attr != null)
                {
                    if (attr.IsSingleInstance)
                        context.Services.AddSingleton(interfaces[0], containerService);
                    else if (attr.IsTransient)
                        context.Services.AddTransient(interfaces[0], containerService);
                    else
                        context.Services.AddScoped(interfaces[0], containerService);
                }
                var attr2 = containerService.GetCustomAttribute<Griffin.Container.ContainerServiceAttribute>();
                if (attr2 == null)
                    continue;

                switch (attr2.Lifetime)
                {
                    case ContainerLifetime.SingleInstance:
                        context.Services.AddSingleton(interfaces[0], containerService);
                        break;
                    case ContainerLifetime.Transient:
                        context.Services.AddTransient(interfaces[0], containerService);
                        break;
                    default:
                        context.Services.AddScoped(interfaces[0], containerService);
                        break;
                }
            }
        }

        public void Start(StartContext context)
        {
        }

        public void Stop()
        {
        }
    }
}