using System;
using System.Collections.Generic;

namespace Coderr.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    /// A collection
    /// </summary>
    public class GetCollectionResultItem
    {
        /// <summary>
        /// Properties in the collection (for instance "Url" if this is the HTTP request).
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Date for the report that this collection is for.
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// Id of the report that this collection was received in.
        /// </summary>
        public int ReportId { get; set; }
    }
}