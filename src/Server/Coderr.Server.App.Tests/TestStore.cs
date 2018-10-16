using System;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;

namespace Coderr.Server.App.Tests
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