using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot
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
                Debug.WriteLine(Assembly.GetCallingAssembly().GetName().Name + " registers " + containerService.FullName);

                var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                var interfaces = containerService.GetInterfaces();
                var lifetime = ConvertLifetime(attr);

                // Hack so that the same instance is resolved for each interface
                bool isRegisteredAsSelf = false;
                if (interfaces.Length > 1 || attr.RegisterAsSelf)
                {
                    serviceCollection.Add(new ServiceDescriptor(containerService, containerService, lifetime));
                    isRegisteredAsSelf = true;
                }

                foreach (var @interface in interfaces)
                {
                    var sd = isRegisteredAsSelf
                        ? new ServiceDescriptor(@interface, x => x.GetService(containerService), lifetime) // else we don't get the same instance in the scope.
                        : new ServiceDescriptor(@interface, containerService, lifetime);
                    serviceCollection.Add(sd);
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
                Debug.WriteLine(Assembly.GetCallingAssembly().GetName().Name + " registers " + type.FullName);

                if (type.GetCustomAttributes().Any(x => x.GetType().Name.StartsWith("ContainerService")))
                {
                    Debugger.Break();
                }

                serviceCollection.AddScoped(type, type);
                serviceCollection.AddScoped(type.GetInterfaces()[0], x => x.GetService(type));
            }
        }



        private static ServiceLifetime ConvertLifetime(ContainerServiceAttribute attr)
        {
            if (attr.IsSingleInstance)
            {
                return ServiceLifetime.Singleton;
            }

            if (attr.IsTransient)
            {
                return ServiceLifetime.Transient;
            }

            return ServiceLifetime.Scoped;
        }
    }
}