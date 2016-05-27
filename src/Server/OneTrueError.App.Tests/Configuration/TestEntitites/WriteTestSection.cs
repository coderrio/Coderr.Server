using System.Collections.Generic;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Tests.Configuration.TestEntitites
{
    internal class WriteTestSection : IConfigurationSection
    {
        public WriteTestSection()
        {
            Items = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Items { get; private set; }

        public string SectionName
        {
            get { return "WriteTest"; }
        }

        public IDictionary<string, string> ToDictionary()
        {
            return Items;
        }

        public void Load(IDictionary<string, string> settings)
        {
            Items = settings;
        }
    }
}