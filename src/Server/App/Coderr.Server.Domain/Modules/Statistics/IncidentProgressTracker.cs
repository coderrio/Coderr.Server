using System;

namespace Coderr.Server.Domain.Modules.Statistics
{
    /// <summary>
    /// An aggregated view of an incident, so that we can calculate statistics in an easier way.
    /// </summary>
    public class IncidentProgressTracker
    {
        public int IncidentId { get; set; }
        public int ApplicationId { get; set; }
        /// <summary>
        ///     date only when the incident was created
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        public int AssignedToId { get; set; }

        /// <summary>
        ///     Date only when the incident was assigned
        /// </summary>
        public DateTime? AssignedAtUtc { get; set; }



        public int ClosedById { get; set; }

        /// <summary>
        ///     Date only when the incident was closed
        /// </summary>
        public DateTime? ClosedAtUtc { get; set; }


        /// <summary>
        ///     Number of times this incident has been reopened.
        /// </summary>
        public int ReOpenCount { get; set; }

        /// <summary>
        ///     Date only when the incident was reopened
        /// </summary>
        public DateTime? ReOpenedAtUtc { get; set; }

        /// <summary>
        ///     Number of versions that this incident appeared in
        /// </summary>
        public int VersionCount { get; set; }

        /// <summary>
        ///     Version strings separated with colons (prefixed and suffixed with colon too)
        /// </summary>
        public string Versions { get; set; }

        public override string ToString()
        {
            var assigned = AssignedAtUtc == null
                ? ""
                : $" Assigned {AssignedToId} at {AssignedAtUtc}";
            var closed = ClosedAtUtc == null
                ? ""
                : $" Closed {ClosedById} at {ClosedAtUtc}, ";
            var reopened = ReOpenedAtUtc == null
                ? ""
                : $" ReOpened {ReOpenedAtUtc} {ReOpenCount} times";

            return $"{CreatedAtUtc.ToShortDateString()} {IncidentId} {ApplicationId} {assigned}{closed}{reopened}";
        }
    }
}