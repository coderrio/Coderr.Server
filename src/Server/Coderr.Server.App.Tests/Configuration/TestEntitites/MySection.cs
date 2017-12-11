using System.Collections.Generic;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using FluentAssertions.Execution;

namespace codeRR.Server.App.Tests.Configuration.TestEntitites
{
    internal class MySection : IConfigurationSection
    {
        public string Name { get; private set; }

        public string SectionName
        {
            get { return "DemoCategory"; }
        }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string> {{"Name", Name}};
        }

        public void Load(IDictionary<string, string> settings)
        {
            Name = settings["Name"];
            if (settings.Count != 1)
                throw new AssertionFailedException("Expected only one setting.");
        }
    }
}