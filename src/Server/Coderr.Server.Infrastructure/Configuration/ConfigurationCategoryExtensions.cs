using Coderr.Server.PluginApi.Config;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace codeRR.Server.Infrastructure.Configuration
{
    /// <summary>
    ///     Extensions used to convert between a flat object and a configuration dictionary
    /// </summary>
    public static class ConfigurationCategoryExtensions
    {
        /// <summary>
        ///     Assign properties from the configuration dictionary.
        /// </summary>
        /// <param name="section">Category that should get its properties assigned</param>
        /// <param name="settings">Dictionary containing the property values.</param>
        /// <exception cref="ArgumentNullException">section;settings</exception>
        public static void AssignProperties(this IConfigurationSection section, IDictionary<string, string> settings)
        {
            if (section == null) throw new ArgumentNullException("section");
            if (settings == null) throw new ArgumentNullException("settings");
            var type = section.GetType();
            foreach (var kvp in settings)
            {
                var property = type.GetProperty(kvp.Key);
                var propertyType = property.PropertyType;
                if (propertyType == typeof(Uri))
                {
                    var value = new Uri(kvp.Value);
                    property.SetValue(section, value);
                }
                else if (!propertyType.IsAssignableFrom(typeof(string)))
                {
                    var realType = Nullable.GetUnderlyingType(propertyType);
                    if (realType != null)
                    {
                        // we got a nullable type and the string represents
                        // null, so just assign it.
                        if (string.IsNullOrEmpty(kvp.Value))
                        {
                            property.SetValue(section, null);
                            continue;
                        }

                        propertyType = realType;
                    }

                    var value = Convert.ChangeType(kvp.Value, propertyType);
                    property.SetValue(section, value);
                }
                else
                    property.SetValue(section, kvp.Value);
            }
        }

        /// <summary>
        ///     Create a dictionary from the objects properties.
        /// </summary>
        /// <param name="section">Instance to convert</param>
        /// <returns>Dictionary with all properties (except <c>SectionName</c>)</returns>
        /// <exception cref="ArgumentNullException">section</exception>
        public static IDictionary<string, string> ToConfigDictionary(this IConfigurationSection section)
        {
            if (section == null) throw new ArgumentNullException("section");
            var items = new Dictionary<string, string>();
            foreach (var propertyInfo in section.GetType().GetProperties())
            {
                if (propertyInfo.Name == "SectionName")
                    continue;

                var value = propertyInfo.GetValue(section);
                items[propertyInfo.Name] = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            }
            return items;
        }
    }
}