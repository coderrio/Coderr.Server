using System;
using System.Configuration;
using Griffin.ApplicationServices;

namespace codeRR.Server.Web.Services
{
    /// <summary>
    ///     The default <c>AppConfigServiceSettings</c> will report off if the key is missing. We want the opposite.
    /// </summary>
    internal class ApplicationServiceManagerSettingsWithDefaultOn : ISettingsRepository
    {
        /// <inheritdoc />
        public bool IsEnabled(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var value = ConfigurationManager.AppSettings[type.Name + ".Enabled"] ?? "true";
            return value == "true";
        }
    }
}