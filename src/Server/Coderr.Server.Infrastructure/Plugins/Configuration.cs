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

        public abstract void ConfigureServices(IServiceCollection services);
    }
}