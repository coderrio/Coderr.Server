using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class ConfigurationSectionWrapper : IConfigurationSection
    {
        private readonly Microsoft.Extensions.Configuration.IConfigurationSection _section;

        public ConfigurationSectionWrapper(Microsoft.Extensions.Configuration.IConfigurationSection section)
        {
            _section = section;
        }

        public string this[string name] => _section[name];

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _section.GetChildren().Select(x => new ConfigurationSectionWrapper(x));
        }

        public string Value => _section.Value;
    }
}