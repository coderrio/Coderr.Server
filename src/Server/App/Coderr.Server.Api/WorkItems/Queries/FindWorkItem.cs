using System;
using DotNetCqs;

namespace Coderr.Server.Api.WorkItems.Queries
{
    /// <summary>
    ///     Find work item for the given incident.
    /// </summary>
    [Message]
    public class FindWorkItem : Query<FindWorkItemResult>
    {
        public FindWorkItem(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
        }

        protected FindWorkItem()
        {
        }

        public int IncidentId { get; private set; }
    }
}