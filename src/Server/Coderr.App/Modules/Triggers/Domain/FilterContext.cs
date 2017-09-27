using codeRR.Server.Api.Core.Incidents;
using codeRR.Server.Api.Core.Reports;

namespace codeRR.Server.App.Modules.Triggers.Domain
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