using Coderr.Server.PluginApi.Config;

namespace Coderr.Server.Web2.Boot.Adapters
{
    public class ConfigWrapper<TConfigType> : IConfiguration<TConfigType>
        where TConfigType : IConfigurationSection, new()
    {
        private readonly ConfigurationStore _configurationStore;

        public ConfigWrapper(ConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public TConfigType Value => _configurationStore.Load<TConfigType>();
    }
}