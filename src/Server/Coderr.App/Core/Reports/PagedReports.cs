using System.Collections.Generic;
using codeRR.Api.Core.Reports;

namespace codeRR.App.Core.Reports
{
    /// <summary>
    ///     Repository result
    /// </summary>
    /// <see cref="IReportsRepository" />
    public class PagedReports
    {
        /// <summary>
        ///     Reports
        /// </summary>
        public IReadOnlyList<ReportDTO> Reports { get; set; }

        /// <summary>
        ///     Total count (the report collection can be paged)
        /// </summary>
        public int TotalCount { get; set; }
    }
}