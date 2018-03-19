using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;

namespace Coderr.Server.ReportAnalyzer.Triggers
{
    /// <summary>
    ///     Execution information for a trigger action.
    /// </summary>
    public class ActionExecutionContext
    {
        /// <summary>
        ///     Config for this action step
        /// </summary>
        public ActionConfigurationData Config { get; set; }

        /// <summary>
        ///     Report that the trigger is running on.
        /// </summary>
        public ReportDTO ErrorReport { get; set; }

        /// <summary>
        ///     Incident that the trigger is runnning for.
        /// </summary>
        public IncidentSummaryDTO Incident { get; set; }
    }
}