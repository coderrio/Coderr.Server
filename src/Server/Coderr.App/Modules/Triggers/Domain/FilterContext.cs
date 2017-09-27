using codeRR.Api.Core.Incidents;
using codeRR.Api.Core.Reports;

namespace codeRR.App.Modules.Triggers.Domain
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