using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Abstractions.Boot
{
    public static class RegisterExtensions
    {

        public static void RegisterContainerServices(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var gotWrongAttribute = (
                from type in assembly.GetTypes()
                let attributes = type.GetCustomAttributes()
                where attributes.Count(x => x.GetType().FullName == "Griffin.Container.ContainerService") > 0
                select type).ToList();
            if (gotWrongAttribute.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Types \'{string.Join(",", gotWrongAttribute)}\' was decorated with the wrong attribute");
            }

            var containerServices = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<ContainerServiceAttribute>() != null);

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
                    isRegisteredAsSelf=true;
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
                .Where(y => y.GetInterfaces()
                    .Any(x => x.Name.Contains("IMessageHandler") || x.Name.Contains("IQueryHandler")))
                .ToList();
            foreach (var type in types)
            {
                Debug.WriteLine(Assembly.GetCallingAssembly().GetName().Name + " registers " + type.FullName);

                serviceCollection.AddScoped(type, type);

                var ifs = type.GetInterfaces()
                    .Where(x => x.Name.Contains("IMessageHandler") || x.Name.Contains("IQueryHandler"))
                    .ToList();
                foreach (var @if in ifs)
                {
                    serviceCollection.AddScoped(@if, x => x.GetService(type));
                }
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