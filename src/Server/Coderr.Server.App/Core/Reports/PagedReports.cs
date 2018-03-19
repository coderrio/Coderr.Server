using System.Collections.Generic;
using Coderr.Server.Api.Core.Reports;

namespace Coderr.Server.App.Core.Reports
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