using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     Ignore incident
    /// </summary>
    [Message]
    public class IgnoreIncident
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IgnoreIncident" />.
        /// </summary>
        /// <param name="incidentId">incident id</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public IgnoreIncident(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }


        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected IgnoreIncident()
        {
        }

        /// <summary>
        ///     Incident to ignore
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Person that ignored the report.
        /// </summary>
        public int UserId { get; set; }
    }
}