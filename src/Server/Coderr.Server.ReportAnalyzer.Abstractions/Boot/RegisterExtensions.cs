using System;
using System.Diagnostics;
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
                var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                var interfaces = containerService.GetInterfaces();

                // Hack so that the same instance is resolved for each interface
                if (interfaces.Length > 1 || attr.RegisterAsSelf)
                    serviceCollection.RegisterService(attr, containerService, containerService);

                foreach (var @interface in interfaces)
                {
                    serviceCollection.RegisterService(attr, @interface, containerService);
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
                if (type.GetCustomAttributes().Any(x => x.GetType().Name.StartsWith("ContainerService")))
                    Debugger.Break();

                serviceCollection.AddScoped(type, type);
                serviceCollection.AddScoped(type.GetInterfaces()[0], type);
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
    }
}