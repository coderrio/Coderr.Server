using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class ConfigWrapper<TConfigType> : IConfiguration<TConfigType>
        where TConfigType : IConfigurationSection, new()
    {
        private readonly ConfigurationStore _configurationStore;
        private TConfigType _value = default(TConfigType);

        public ConfigWrapper(ConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public TConfigType Value
        {
            get
            {
                if (EqualityComparer<TConfigType>.Default.Equals(_value, default(TConfigType)))
                    _value = _configurationStore.Load<TConfigType>();

                return _value;
            }
        }

        public void Save()
        {
            _configurationStore.Store(Value);
        }
    }
}