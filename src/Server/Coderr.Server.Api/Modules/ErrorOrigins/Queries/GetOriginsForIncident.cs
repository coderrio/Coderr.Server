using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.ErrorOrigins.Queries
{
    /// <summary>
    ///     Get all error origins for the specified incident.
    /// </summary>
    [Message]
    public class GetOriginsForIncident : Query<GetOriginsForIncidentResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetOriginsForIncident" />.
        /// </summary>
        /// <param name="incidentId">incident to get error origins for</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId &lt; 1</exception>
        public GetOriginsForIncident(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetOriginsForIncident()
        {
        }

        /// <summary>
        ///     Incident to get origins for
        /// </summary>
        public int IncidentId { get; private set; }
    }
}