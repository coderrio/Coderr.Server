using System;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.App.Tests
{
    public class TestStore : ConfigurationStore
    {
        public override T Load<T>()
        {
            if (typeof(T) == typeof(BaseConfiguration))
                return (T) (object) new BaseConfiguration {BaseUrl = new Uri("http://localhost/")};

            return new T();
        }

        public override void Store(IConfigurationSection section)
        {
        }
    }
}