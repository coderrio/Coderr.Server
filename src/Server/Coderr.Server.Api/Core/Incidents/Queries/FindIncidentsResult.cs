using System.Collections.Generic;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Result for <see cref="FindIncidentsResult" />.
    /// </summary>
    public class FindIncidentsResult
    {
        /// <summary>
        ///     Items
        /// </summary>
        public IReadOnlyList<FindIncidentsResultItem> Items { get; set; }

        /// <summary>
        ///     Page number (one based index)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///     Items returned for this page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Total number of items
        /// </summary>
        public int TotalCount { get; set; }
    }
}