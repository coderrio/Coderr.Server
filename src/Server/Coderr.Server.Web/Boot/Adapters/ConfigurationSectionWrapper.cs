using Coderr.Server.Infrastructure.Boot;

namespace Coderr.Server.Web2.Boot.Adapters
{
    public class ConfigurationSectionWrapper : IConfigurationSection
    {
        private readonly Microsoft.Extensions.Configuration.IConfigurationSection _section;

        public ConfigurationSectionWrapper(Microsoft.Extensions.Configuration.IConfigurationSection section)
        {
            _section = section;
        }

        public string this[string name] => _section[name];
    }
}