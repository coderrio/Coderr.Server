using System.Collections.Generic;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Tests.Configuration.TestEntitites
{
    internal class WriteTestSection : IConfigurationSection
    {
        public WriteTestSection()
        {
            Properties = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Properties { get; private set; }

        public string SectionName
        {
            get { return "WriteTest"; }
        }

        public IDictionary<string, string> ToDictionary()
        {
            return Properties;
        }

        public void Load(IDictionary<string, string> settings)
        {
            Properties = settings;
        }
    }
}