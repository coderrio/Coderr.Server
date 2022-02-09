using System.Collections.Generic;
using System.Linq;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    public class ServerConfigWrapper : IConfiguration
    {
        private readonly Server.Abstractions.Boot.IConfiguration _inner;

        public ServerConfigWrapper(Server.Abstractions.Boot.IConfiguration inner)
        {
            _inner = inner;
        }

        public string this[string name] => _inner[name];

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _inner.GetChildren().Select(x => new ServerConfigSectionWrapper(x));
        }

        public IConfigurationSection GetSection(string name)
        {
            return new ServerConfigSectionWrapper(_inner.GetSection(name));
        }

        public string GetConnectionString(string name)
        {
            return _inner.GetConnectionString(name);
        }
    }
}