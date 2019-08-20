using System;

namespace Coderr.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     Delete incidents (old, created in dev environment etc)
    /// </summary>
    [Message]
    public class DeleteIncident
    {
        /// <summary>
        ///     Creates a new instance of <see cref="DeleteIncident" />.
        /// </summary>
        /// <param name="incidentId">incident to delete</param>
        /// <param name="areYouSure">should be "yes"</param>
        public DeleteIncident(int incidentId, string areYouSure)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (areYouSure != "yes")
                throw new ArgumentOutOfRangeException(nameof(areYouSure), areYouSure, "Must be 'yes'.");
            IncidentId = incidentId;
            AreYouSure = areYouSure;
        }

        public int IncidentId { get; private set; }
        public int? UserId { get; set; }

        public DateTime? DeletedAtUtc { get; set; }
        public string AreYouSure { get; private set; }
    }
}