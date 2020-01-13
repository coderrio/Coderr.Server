using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.Abstractions.Reports
{
    /// <summary>
    ///     Configuration settings for reports.
    /// </summary>
    public class ReportConfig : IConfigurationSection//, IReportConfig
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ReportConfig" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Sets default of MaxReportJsonSize to 2000000, MaxReportsPerIncident to 25 and RetentionDays to 90.
        ///     </para>
        /// </remarks>
        public ReportConfig()
        {
            MaxReportJsonSize = 2000000;
            MaxReportsPerIncident = 25;
            RetentionDays = 90;
        }

        /// <summary>
        ///     Maximum number of bytes that a uncompressed JSON report can be
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used to filter out reports that are too large.
        ///     </para>
        /// </remarks>
        public int MaxReportJsonSize { get; set; }

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

        string IConfigurationSection.SectionName => "ReportConfig";

        IDictionary<string, string> IConfigurationSection.ToDictionary()
        {
            return this.ToConfigDictionary();
        }

        void IConfigurationSection.Load(IDictionary<string, string> settings)
        {
            this.AssignProperties(settings);
            if (MaxReportJsonSize == 0)
                MaxReportJsonSize = 1000000;
        }
    }
}