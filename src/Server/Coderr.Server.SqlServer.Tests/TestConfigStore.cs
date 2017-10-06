using codeRR.Server.Infrastructure.Configuration;

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