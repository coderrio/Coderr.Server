using System;

namespace Coderr.Server.PluginApi.Config
{
    /// <summary>
    ///     Used to modify configuration settings.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Use dependency injection (<see cref="IConfiguration{T}" />) to access the configuration.
    ///     </para>
    /// </remarks>
    public abstract class ConfigurationStore
    {
        /// <summary>
        ///     Load a settings section
        /// </summary>
        /// <typeparam name="T">Type of section</typeparam>
        /// <returns>Category if found; otherwise <c>null</c>.</returns>
        public abstract T Load<T>() where T : IConfigurationSection, new();

        /// <summary>
        ///     Store a settings section.
        /// </summary>
        /// <param name="section">Category to persist.</param>
        /// <exception cref="ArgumentNullException">section</exception>
        public abstract void Store(IConfigurationSection section);
    }
}