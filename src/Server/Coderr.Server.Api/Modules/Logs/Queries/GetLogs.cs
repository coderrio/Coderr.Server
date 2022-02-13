using System;
using DotNetCqs;

namespace Coderr.Server.Api.Modules.Logs.Queries
{
    [Message]
    public class GetLogs : Query<GetLogsResult>
    {
        public GetLogs(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
        }

        public GetLogs(int incidentId, int reportId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (reportId <= 0) throw new ArgumentOutOfRangeException(nameof(reportId));
            IncidentId = incidentId;
            ReportId = reportId;
        }

        protected GetLogs()
        {

        }

        /// <summary>
        /// Incident to check.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        /// Check for a specific report (if set).
        /// </summary>
        public int? ReportId { get; private set; }
    }
}
