using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;

namespace Coderr.Server.App.Tests.Configuration.TestEntitites
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