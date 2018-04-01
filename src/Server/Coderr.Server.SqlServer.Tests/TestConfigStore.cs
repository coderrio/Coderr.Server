using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.SqlServer.Tests
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