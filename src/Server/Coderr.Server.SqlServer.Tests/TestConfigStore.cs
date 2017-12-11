using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.SqlServer.Tests
{
    public class TestConfigStore : ConfigurationStore
    {
        public override T Load<T>()
        {
            return new T();
        }

        public override void Store(IConfigurationSection section)
        {
            
        }
    }
}