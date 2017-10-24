using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Get incident query
    /// </summary>
    [Message]
    public class GetIncident : Query<GetIncidentResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetIncident" />.
        /// </summary>
        /// <param name="incidentId">incident id</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetIncident(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }


        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetIncident()
        {
        }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int IncidentId { get; private set; }
    }
}