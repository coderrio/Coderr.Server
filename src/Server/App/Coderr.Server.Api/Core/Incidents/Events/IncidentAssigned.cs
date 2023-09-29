using System;

namespace Coderr.Server.Api.Core.Incidents.Events
{
    /// <summary>
    ///     Someone was assigned to an incident
    /// </summary>
    [Event]
    public class IncidentAssigned
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentAssigned" />.
        /// </summary>
        /// <param name="incidentId">Incident being assigned</param>
        /// <param name="assignedById">User assigning the incident</param>
        /// <param name="assignedToId">User that should start working with the incident</param>
        /// <param name="assignedAtUtc">When incident was assigned</param>
        public IncidentAssigned(int incidentId, int assignedById, int assignedToId, DateTime assignedAtUtc)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (assignedById <= 0) throw new ArgumentOutOfRangeException(nameof(assignedById));
            if (assignedToId <= 0) throw new ArgumentOutOfRangeException(nameof(assignedToId));

            IncidentId = incidentId;
            AssignedById = assignedById;
            AssignedToId = assignedToId;
            AssignedAtUtc = assignedAtUtc;
        }

        protected IncidentAssigned()
        {

        }

        /// <summary>
        ///     User assigning the incident (delegate work)
        /// </summary>
        public int AssignedById { get; private set; }

        /// <summary>
        ///     User that should start working with the incident
        /// </summary>
        public int AssignedToId { get; private set; }

        /// <summary>
        /// When the incident was assigned (client side)
        /// </summary>
        public DateTime AssignedAtUtc { get; private set; }

        /// <summary>
        ///     Incident being assigned
        /// </summary>
        public int IncidentId { get; private set; }
    }
}