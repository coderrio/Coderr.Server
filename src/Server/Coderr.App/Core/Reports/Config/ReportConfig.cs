using System.Collections.Generic;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Core.Reports.Config
{
    /// <summary>
    ///     Configuration settings for reports.
    /// </summary>
    public class ReportConfig : IConfigurationSection
    {
        /// <summary>
        ///     Max number of reports per incident
        /// </summary>
        /// <remarks>
        ///     The oldest report(s) will be deleted if this limit is reached.
        /// </remarks>
        public int MaxReportsPerIncident { get; set; }

        /// <summary>
        ///     Number of days to store reports.
        /// </summary>
        /// <remarks>
        ///     All reports older than this amount of days will be deleted.
        /// </remarks>
        public int RetentionDays { get; set; }


        string IConfigurationSection.SectionName
        {
            get { return "ReportConfig"; }
        }

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