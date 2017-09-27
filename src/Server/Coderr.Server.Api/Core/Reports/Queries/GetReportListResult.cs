using System;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Result for <see cref="GetReportList" />.
    /// </summary>
    public class GetReportListResult
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetReportListResult" />.
        /// </summary>
        /// <param name="items">Result items</param>
        /// <exception cref="ArgumentNullException">items</exception>
        public GetReportListResult(GetReportListResultItem[] items)
        {
            if (items == null) throw new ArgumentNullException("items");
            Items = items;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetReportListResult()
        {
        }

        /// <summary>
        ///     Items on this page.
        /// </summary>
        public GetReportListResultItem[] Items { get; set; }

        /// <summary>
        ///     Page number being returned
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///     Number of items on this page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Total number of items that a non-paged query would return
        /// </summary>
        public int TotalCount { get; set; }
    }
}