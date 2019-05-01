using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Incidents.Events
{
    [Event]
    public class IncidentClosed
    {
        public IncidentClosed(int incidentId, int userId, string solution, string applicationVersion, DateTime closedAtUtc)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));

            IncidentId = incidentId;
            ClosedById = userId;
            Solution = solution;
            ApplicationVersion = applicationVersion;
            ClosedAtUtc = closedAtUtc;
        }

        protected IncidentClosed()
        {

        }

        public int ClosedById { get; private set; }
        public int IncidentId { get;private  set; }
        public string Solution { get;private  set; }
        public string ApplicationVersion { get; private set; }
        public DateTime ClosedAtUtc { get; }
    }
}
