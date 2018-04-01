using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Abstractions.Boot
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
                var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                var interfaces = containerService.GetInterfaces();

                // Hack so that the same instance is resolved for each interface
                if (interfaces.Length > 1)
                    serviceCollection.RegisterService(attr, containerService, containerService);

                foreach (var @interface in interfaces)
                {
                    serviceCollection.RegisterService(attr, @interface, containerService);
                }

            }
        }

        private static void RegisterService(this IServiceCollection serviceCollection, ContainerServiceAttribute attr,
            Type service, Type implementation)
        {
            if (attr.IsSingleInstance)
                serviceCollection.AddSingleton(service, implementation);
            else if (attr.IsTransient)
                serviceCollection.AddTransient(service, implementation);
            else
                serviceCollection.AddScoped(service, implementation);
        }

        public static void RegisterMessageHandlers(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(y => y.GetInterfaces().Any(x => x.Name.Contains("IMessageHandler") || x.Name.Contains("IQueryHandler")))
                .ToList();
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type, type);
                serviceCollection.AddScoped(type.GetInterfaces()[0], type);
            }
        }

    }
}
