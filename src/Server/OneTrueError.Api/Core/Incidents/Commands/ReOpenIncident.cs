using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     An incident which has either been closed or ignored is marked as active again
    /// </summary>
    public class ReOpenIncident : Command
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
        public int IncidentId { get; }

        /// <summary>
        ///     User requesting item to be reopened.
        /// </summary>
        public int UserId { get; set; }
    }
}