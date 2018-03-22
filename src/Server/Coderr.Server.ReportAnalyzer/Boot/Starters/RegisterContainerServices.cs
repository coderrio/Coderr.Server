using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Coderr.Server.Infrastructure.Boot;
using Griffin.Container;
using Microsoft.Extensions.DependencyInjection;
using ContainerServiceAttribute = Coderr.Server.ReportAnalyzer.Abstractions.ContainerServiceAttribute;

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    public class RegisterContainerServices 
    {
        public void Configure(ConfigurationContext context)
        {
            RegisterAssemblyServices(Assembly.GetExecutingAssembly(), context);

            //workaround since SQL server already references us
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .First(x => x.FullName.StartsWith("Coderr.Server.SqlServer,"));

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

                foreach (var @interface in interfaces)
                {
                    var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                    if (attr != null)
                    {
                        if (attr.IsSingleInstance)
                            context.Services.AddSingleton(@interface, containerService);
                        else if (attr.IsTransient)
                            context.Services.AddTransient(@interface, containerService);
                        else
                            context.Services.AddScoped(@interface, containerService);
                    }
                    var attr2 = containerService.GetCustomAttribute<Griffin.Container.ContainerServiceAttribute>();
                    if (attr2 == null)
                        continue;

                    switch (attr2.Lifetime)
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