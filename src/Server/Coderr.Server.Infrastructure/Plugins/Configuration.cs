using System;
using System.Reflection;

namespace codeRR.Server.Infrastructure.Plugins
{
    /// <summary>
    ///     Configuration possibilities for plugins
    /// </summary>
    public abstract class Configuration
    {
        protected Configuration()
        {
            Menu = new MenuConfiguration();
        }

        /// <summary>
        ///     Add items to the different menus.
        /// </summary>
        public MenuConfiguration Menu { get; }


        /// <summary>
        ///     Register all handles in the assembly that implement the interfaces in DotNetCqs.
        /// </summary>
        /// <param name="assembly"></param>
        public abstract void RegisterCqrsHandlers(Assembly assembly);

        /// <summary>
        ///     Register a service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factoryMethod"></param>
        public abstract void RegisterService<TService>(Func<IScopedServiceLocator, TService> factoryMethod);

        /// <summary>
        ///     Register services using the <c>[Component]</c> attribute from Griffin.Container.
        /// </summary>
        /// <param name="assembly"></param>
        public abstract void RegisterUsingComponentAttribute(Assembly assembly);
    }
}