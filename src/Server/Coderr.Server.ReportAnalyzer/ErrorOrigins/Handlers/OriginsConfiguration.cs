using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;

namespace Coderr.Server.ReportAnalyzer.ErrorOrigins.Handlers
{
    public class OriginsConfiguration : IConfigurationSection
    {
        public string ApiKey { get; set; }

        string IConfigurationSection.SectionName { get; } = "Origins";

        IDictionary<string, string> IConfigurationSection.ToDictionary()
        {
            return this.ToConfigDictionary();
        }

        void IConfigurationSection.Load(IDictionary<string, string> settings)
        {
            this.AssignProperties(settings);
        }
    }
}