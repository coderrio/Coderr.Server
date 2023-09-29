using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;

namespace Coderr.Server.ReportAnalyzer.Triggers
{
    /// <summary>
    ///     Context providing when the trigger should execute
    /// </summary>
    public class TriggerExecutionContext
    {
        /// <summary>
        ///     Report that triggered the trigger (ha ha)
        /// </summary>
        public ReportDTO ErrorReport { get; set; }

        /// <summary>
        ///     Incident that the received report belongs to
        /// </summary>
        public IncidentSummaryDTO Incident { get; set; }
    }
}