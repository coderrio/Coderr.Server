using System.Linq;
using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public static class RegisterExtensions
    {
        public static void RegisterContainerServices(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var containerServices = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<ContainerServiceAttribute>(true) != null)
                .ToList();
            foreach (var containerService in containerServices)
            {
                var interfaces = containerService.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                    if (attr != null)
                    {
                        if (attr.IsSingleInstance)
                            serviceCollection.AddSingleton(@interface, containerService);
                        else if (attr.IsTransient)
                            serviceCollection.AddTransient(@interface, containerService);
                        else
                            serviceCollection.AddScoped(@interface, containerService);
                    }
                }

            }
        }

        public static void RegisterMessageHandlers(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(y => y.GetInterfaces().Any(x => x.Name.Contains("IMessageHandler")))
                .ToList();
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type, type);
                serviceCollection.AddScoped(type.GetInterfaces()[0], type);
            }
        }

    }
}
