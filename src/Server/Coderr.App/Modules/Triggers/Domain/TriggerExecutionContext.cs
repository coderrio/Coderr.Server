using codeRR.Api.Core.Incidents;
using codeRR.Api.Core.Reports;

namespace codeRR.App.Modules.Triggers.Domain
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