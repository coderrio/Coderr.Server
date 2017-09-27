using System;
using System.Diagnostics.CodeAnalysis;
using codeRR.Server.Infrastructure.Configuration.Database;

namespace codeRR.Server.Infrastructure.Configuration
{
    /// <summary>
    ///     Defines how settings should be persisted and loaded.
    /// </summary>
    public abstract class ConfigurationStore
    {
        /// <summary>
        ///     Used to access the currently configured implementation
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")] public static
            ConfigurationStore Instance = new DatabaseStore();

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