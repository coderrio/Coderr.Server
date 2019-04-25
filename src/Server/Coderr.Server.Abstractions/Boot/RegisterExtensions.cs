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
            var containerServices = assembly.GetTypes();
            foreach (var containerService in containerServices)
            {
                var gotWrongAttribute = containerService
                    .GetCustomAttributes()
                    .Count(x => x.GetType().FullName == "Griffin.Container.ContainerService") == 1;
                if (gotWrongAttribute)
                {
                    throw new InvalidOperationException(
                        $"Type \'{containerService}\' was decorated with the wrong attribute");
                    ;
                }

                var attr = containerService.GetCustomAttribute<ContainerServiceAttribute>();
                if (attr == null)
                    continue;

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
                .Where(y => y.GetInterfaces()
                    .Any(x => x.Name.Contains("IMessageHandler") || x.Name.Contains("IQueryHandler")))
                .ToList();
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type, type);

                var ifs = type.GetInterfaces()
                    .Where(x => x.Name.Contains("IMessageHandler") || x.Name.Contains("IQueryHandler"))
                    .ToList();
                foreach (var @if in ifs)
                {
                    serviceCollection.AddScoped(@if, type);
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
    }
}