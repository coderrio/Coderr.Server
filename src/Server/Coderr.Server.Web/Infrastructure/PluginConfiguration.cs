using System;
using System.Linq;
using System.Reflection;
using codeRR.Server.Infrastructure.Plugins;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.Web.Infrastructure
{
    public class PluginConfiguration : Configuration
    {
        private readonly IContainerRegistrar _containerRegistrar;

        public PluginConfiguration(IContainerRegistrar containerRegistrar)
        {
            _containerRegistrar = containerRegistrar;
        }

        public override void RegisterCqrsHandlers(Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                let handlerInterface = GetHandlerInterface(type)
                where handlerInterface != null
                select new {type, handlerInterface};
            foreach (var kvp in types)
            {
                _containerRegistrar.RegisterType(kvp.handlerInterface, kvp.type, Lifetime.Scoped);
                _containerRegistrar.Registrations.First(x => x.ConcreteType == kvp.type).AddService(kvp.type);
            }
        }

        public override void RegisterService<TService>(Func<IScopedServiceLocator, TService> factoryMethod)
        {
            _containerRegistrar.RegisterService(x => factoryMethod(new ServiceLocatorAdapter(x)), Lifetime.Scoped);
        }

        public override void RegisterUsingComponentAttribute(Assembly assembly)
        {
            _containerRegistrar.RegisterComponents(Lifetime.Scoped, assembly);
        }

        private Type GetHandlerInterface(Type type)
        {
            var matchingInterfaces = from ifType in type.GetInterfaces()
                where ifType.IsGenericType
                let typeDefinition = ifType.GetGenericTypeDefinition()
                where typeDefinition == typeof(IMessageHandler<>)
                      || typeDefinition == typeof(IMessageHandler<>)
                      || typeDefinition == typeof(IQueryHandler<,>)
                select ifType;
            return matchingInterfaces.FirstOrDefault();
        }
    }
}