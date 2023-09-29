using System;

namespace Coderr.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     Start working on an incident.
    /// </summary>
    [Command]
    public class AssignIncident
    {
        /// <summary>
        ///     Creates new instance of <see cref="AssignIncident" />.
        /// </summary>
        /// <param name="incidentId">Incident being assigned</param>
        /// <param name="assignedTo">Id of the user that got assigned to this incident</param>
        /// <param name="assignedBy">Id of the user that assigned this incident, 0 for system requests</param>
        public AssignIncident(int incidentId, int assignedTo, int assignedBy)
        {
            if (assignedTo <= 0) throw new ArgumentOutOfRangeException(nameof(assignedTo));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (assignedBy < 0) throw new ArgumentOutOfRangeException(nameof(assignedBy));

            IncidentId = incidentId;
            AssignedTo = assignedTo;
            AssignedBy = assignedBy;
        }

        /// <summary>
        ///     Id of the user that assigned this incident.
        /// </summary>
        public int AssignedBy { get; private set; }

        /// <summary>
        ///     Id of the user that got assigned to this incident.
        /// </summary>
        public int AssignedTo { get; private set; }

        /// <summary>
        ///     Incident being assigned.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        /// Optionally specify when the incident was assigned. Default = now.
        /// </summary>
        public DateTime? AssignedAtUtc { get; set; }
    }
}