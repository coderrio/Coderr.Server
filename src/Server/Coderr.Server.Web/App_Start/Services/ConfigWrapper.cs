using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.Web.Services
{
    public class ConfigWrapper<TConfigType> : IConfiguration<TConfigType> where TConfigType : IConfigurationSection, new()
    {
        private readonly ConfigurationStore _configurationStore;

        public ConfigWrapper(ConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public TConfigType Value
        {
            get { return _configurationStore.Load<TConfigType>(); }
        }
    }
}