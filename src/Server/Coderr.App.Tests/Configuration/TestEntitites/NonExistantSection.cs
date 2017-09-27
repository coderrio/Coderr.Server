using System.Collections.Generic;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Tests.Configuration.TestEntitites
{
    public class NonExistantSection : IConfigurationSection
    {
        public string SectionName
        {
            get { return "NpNp"; }
        }

        public IDictionary<string, string> ToDictionary()
        {
            return this.ToConfigDictionary();
        }

        public void Load(IDictionary<string, string> settings)
        {
        }
    }
}