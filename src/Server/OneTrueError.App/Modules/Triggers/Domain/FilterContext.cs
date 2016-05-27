using OneTrueError.Api.Core.Incidents;
using OneTrueError.Api.Core.Reports;

namespace OneTrueError.App.Modules.Triggers.Domain
{
    /// <summary>
    /// Context filter.
    /// </summary>
    public class FilterContext
    {
        /// <summary>
        /// Gets incident that the report was attached to.
        /// </summary>
        public IncidentSummaryDTO Incident { get; set; }

        /// <summary>
        /// Ges the received error report
        /// </summary>
        public ReportDTO ErrorReport { get; set; }
    }
}