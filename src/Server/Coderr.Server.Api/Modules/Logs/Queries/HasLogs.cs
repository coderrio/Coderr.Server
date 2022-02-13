using System;
using DotNetCqs;

namespace Coderr.Server.Api.Modules.Logs.Queries
{
    /// <summary>
    /// Check if an incident (or a specific report for that incident) has logs attached to it.
    /// </summary>
    [Message]
    public class HasLogs : Query<HasLogsReply>
    {
        public HasLogs(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
        }

        public HasLogs(int incidentId, int reportId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (reportId <= 0) throw new ArgumentOutOfRangeException(nameof(reportId));
            IncidentId = incidentId;
            ReportId = reportId;
        }

        protected HasLogs()
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