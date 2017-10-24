using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Get reports
    /// </summary>
    [Message]
    public class GetReportList : Query<GetReportListResult>
    {
        /// <summary>
        ///     Get reports
        /// </summary>
        /// <param name="incidentId">incident to get reports for</param>
        public GetReportList(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetReportList()
        {
        }


        /// <summary>
        ///     Incident id.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Page number (one based index)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///     Page size (default is 20).
        /// </summary>
        public int PageSize { get; set; }
    }
}