using Microsoft.Extensions.Configuration;
using IConfiguration = Coderr.Server.Infrastructure.Boot.IConfiguration;
using IConfigurationSection = Coderr.Server.Infrastructure.Boot.IConfigurationSection;

namespace Coderr.Server.Web2.Boot.Adapters
{
    public class ConfigurationWrapper : IConfiguration
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public ConfigurationWrapper(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _config = config;
        }

        public string this[string name] => _config[name];

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