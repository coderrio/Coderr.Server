using System;
using System.Collections.Generic;
using System.Globalization;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.App.Modules.MonthlyStats
{
    public class UsageStatsSettings : IConfigurationSection
    {
        public DateTime? LatestUploadedMonth { get; set; }

        string IConfigurationSection.SectionName => "UsageStats";

        void IConfigurationSection.Load(IDictionary<string, string> settings)
        {
            if (settings.Count == 0)
                return;

            var value = settings["LatestUploadedMonth"];
            LatestUploadedMonth = DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        IDictionary<string, string> IConfigurationSection.ToDictionary()
        {
            if (LatestUploadedMonth == null)
                return new Dictionary<string, string>();

            var value = LatestUploadedMonth.Value.ToString("R", CultureInfo.InvariantCulture);
            return new Dictionary<string, string>
            {
                {"LatestUploadedMonth", value}
            };
        }
    }
}