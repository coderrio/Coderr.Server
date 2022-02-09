using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using IConfiguration = Coderr.Server.Abstractions.Boot.IConfiguration;
using IConfigurationSection = Coderr.Server.Abstractions.Boot.IConfigurationSection;

namespace Coderr.Server.WebSite.Infrastructure.Adapters
{
    public class ConfigurationWrapper : IConfiguration
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public ConfigurationWrapper(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _config = config;
        }

        public string this[string name] => _config[name];

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _config.GetChildren().Select(x => new ConfigurationSectionWrapper(x));
        }

        public IConfigurationSection GetSection(string name)
        {
            var section = _config.GetSection(name);
            return section == null
                ? null
                : new ConfigurationSectionWrapper(section);
        }

        public string GetConnectionString(string name)
        {
            return _config.GetConnectionString(name);
        }
    }
}