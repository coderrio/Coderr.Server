using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;

namespace Coderr.Server.ReportAnalyzer.Triggers
{
    /// <summary>
    ///     Context filter.
    /// </summary>
    public class FilterContext
    {
        /// <summary>
        ///     Ges the received error report
        /// </summary>
        public ReportDTO ErrorReport { get; set; }

        /// <summary>
        ///     Gets incident that the report was attached to.
        /// </summary>
        public IncidentSummaryDTO Incident { get; set; }
    }
}