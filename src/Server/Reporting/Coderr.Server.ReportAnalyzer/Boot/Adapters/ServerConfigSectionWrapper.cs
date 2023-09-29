using System.Collections.Generic;
using System.Linq;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    public class ServerConfigSectionWrapper : IConfigurationSection
    {
        private readonly Server.Abstractions.Boot.IConfigurationSection _inner;

        public ServerConfigSectionWrapper(Server.Abstractions.Boot.IConfigurationSection inner)
        {
            _inner = inner;
        }

        public string this[string name] => _inner[name];

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _inner.GetChildren().Select(x => new ServerConfigSectionWrapper(x));
        }

        public string Value => _inner.Value;
    }
}