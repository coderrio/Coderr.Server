using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.SqlServer;
using Griffin.ApplicationServices;
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
                var interfaces = containerService.GetInterfaces().ToList();
                var griffinAttribute = containerService.GetCustomAttribute<Griffin.Container.ContainerServiceAttribute>();
                var ourAttribute = containerService.GetCustomAttribute<ContainerServiceAttribute>();

                // Register as self 
                // is required to be able to execute them
                if (interfaces.Any(x => typeof(IBackgroundJobAsync).IsAssignableFrom(x) || typeof(IBackgroundJob).IsAssignableFrom(x)))
                    interfaces.Add(containerService);

                foreach (var @interface in interfaces)
                {
                    if (ourAttribute != null)
                    {
                        if (ourAttribute.IsSingleInstance)
                            context.Services.AddSingleton(@interface, containerService);
                        else if (ourAttribute.IsTransient)
                            context.Services.AddTransient(@interface, containerService);
                        else
                            context.Services.AddScoped(@interface, containerService);
                    }
                    if (griffinAttribute == null)
                        continue;

                    switch (griffinAttribute.Lifetime)
                    {
                        case ContainerLifetime.SingleInstance:
                            context.Services.AddSingleton(@interface, containerService);
                            break;
                        case ContainerLifetime.Transient:
                            context.Services.AddTransient(@interface, containerService);
                            break;
                        default:
                            context.Services.AddScoped(@interface, containerService);
                            break;
                    }
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