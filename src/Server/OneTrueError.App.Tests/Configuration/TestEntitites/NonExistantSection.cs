using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Tests.Configuration.TestEntitites
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
