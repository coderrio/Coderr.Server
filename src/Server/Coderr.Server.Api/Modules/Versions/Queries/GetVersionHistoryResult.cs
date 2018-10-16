using System.Collections.Generic;

namespace Coderr.Server.Api.Modules.Versions.Queries
{
    [Message]
    public class GetVersionHistoryResult
    {
        /// <summary>
        /// Year-Month
        /// </summary>
        public string[] Dates { get; set; }

        /// <summary>
        /// Key = version name, Value = number of incidents for the  month
        /// </summary>
        public GetVersionHistoryResultSet[] IncidentCounts { get; set; }

        /// <summary>
        /// Key = version name, Value = number of error reports for the  month
        /// </summary>
        public GetVersionHistoryResultSet[] ReportCounts { get; set; }
    }

    public class GetVersionHistoryResultSet
    {
        public string Name { get; set; }
        public int[] Values { get; set; }
    }
}