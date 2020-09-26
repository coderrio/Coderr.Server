using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Infrastructure.Plugins
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
        /// Register multiple services in the IoC container.
        /// </summary>
        /// <param name="services">Configuration context</param>
        public abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        ///     Register a service
        /// </summary>
        /// <typeparam name="TService">Type of service to register</typeparam>
        public abstract void RegisterInstance<TService>(TService service);
    }
}