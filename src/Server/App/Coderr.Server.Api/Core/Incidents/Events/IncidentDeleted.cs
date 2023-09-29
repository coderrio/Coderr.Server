using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Incidents.Events
{
    [Event]
    public class IncidentDeleted
    {
        public IncidentDeleted(int incidentId, int userId, DateTime deletedAtUtc)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));

            IncidentId = incidentId;
            DeletedById = userId;
            DeletedAtUtc = deletedAtUtc;
        }

        protected IncidentDeleted()
        {

        }

        public int IncidentId { get;private  set; }

        public int DeletedById { get; set; }
        public DateTime DeletedAtUtc { get; private set; }
    }
}
