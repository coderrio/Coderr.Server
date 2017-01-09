using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    ///     Store the configuration in <c>web.config</c>.
    /// </summary>
    public class ConfigFileStore : ConfigurationStore
    {
        /// <summary>
        ///     Load a settings section
        /// </summary>
        /// <typeparam name="T">Type of section</typeparam>
        /// <returns>Category if found; otherwise <c>null</c>.</returns>
        public override T Load<T>()
        {
            var settingsType = new T();
            var dict = new Dictionary<string, string>();
            var config = ConfigurationManager.GetSection("oneTrueError") as OneTrueErrorConfigurationSection;

            var section = config.Sections[settingsType.SectionName];
            if (section == null)
                return default(T);

            foreach (KeyValueElement elem in section.Settings)
            {
                dict[elem.Key] = elem.Value;
            }

            if (dict.Count == 0)
                return default(T);

            settingsType.Load(dict);
            return settingsType;
        }

        /// <summary>
        ///     Store a settings section.
        /// </summary>
        /// <param name="section">Category to persist.</param>
        /// <exception cref="ArgumentNullException">section</exception>
        /// <remarks>
        ///     <para>
        ///         The section name is used as a prefix for the appSetting key. i.e. the setting <c>Name</c> in a section named
        ///         <c>Properties</c> would
        ///         be stored with the key <c>Properties.Name</c>
        ///     </para>
        /// </remarks>
        public override void Store(IConfigurationSection section)
        {
            if (section == null) throw new ArgumentNullException("section");

            var configuration = HttpContext.Current == null
                ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                : WebConfigurationManager.OpenWebConfiguration("~/");

            var config = (OneTrueErrorConfigurationSection) configuration.GetSection("oneTrueError");
            var appConfigSection = config.Sections[section.SectionName];
            if (appConfigSection == null)
            {
                appConfigSection = new SectionConfigElement {Name = section.SectionName};
                config.Sections.Add(appConfigSection);
            }

            var props = section.ToDictionary();
            foreach (var kvp in props)
            {
                var configItem = appConfigSection.Settings[kvp.Key];
                if (configItem == null)
                {
                    configItem = new KeyValueElement(kvp.Key, kvp.Value);
                    appConfigSection.Settings.Add(configItem);
                }
                else
                    appConfigSection.Settings[kvp.Key].Value = kvp.Value;
            }


            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}