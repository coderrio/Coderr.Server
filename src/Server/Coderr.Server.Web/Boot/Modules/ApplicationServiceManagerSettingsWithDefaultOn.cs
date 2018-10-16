using System;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;

namespace Coderr.Server.Web.Boot.Modules
{
    /// <summary>
    ///     The default <c>AppConfigServiceSettings</c> will report off if the key is missing. We want the opposite.
    /// </summary>
    internal class ApplicationServiceManagerSettingsWithDefaultOn : ISettingsRepository
    {
        private readonly IConfigurationSection _configSection;

        public ApplicationServiceManagerSettingsWithDefaultOn(IConfigurationSection configSection)
        {
            _configSection = configSection;
        }

        /// <inheritdoc />
        public bool IsEnabled(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var value = _configSection[type.Name + ".Enabled"] ?? "true";
            return value == "true";
        }
    }
}