using System;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace codeRR.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     An incident which has either been closed or ignored is marked as active again
    /// </summary>
    [Message]
    public class ReOpenIncident
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ReOpenIncident" />.
        /// </summary>
        /// <param name="incidentId">incident to reopen</param>
        public ReOpenIncident(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ReOpenIncident()
        {
        }

        /// <summary>
        ///     Incident to reopen
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     User requesting item to be reopened.
        /// </summary>
        public int UserId { get; set; }
    }
}