using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;

namespace Coderr.Server.App.IncidentInsights
{
    public class InsightConfiguration : IConfigurationSection
    {
        public string SectionName => "Insights";

        public bool HaveFilledDatabase { get; set; }

        public void Load(IDictionary<string, string> settings)
        {
            HaveFilledDatabase = settings.GetBoolean("HaveFilledDatabase", false);
        }

        public IDictionary<string, string> ToDictionary()
        {
            return this.ToConfigDictionary();
        }
    }
}
