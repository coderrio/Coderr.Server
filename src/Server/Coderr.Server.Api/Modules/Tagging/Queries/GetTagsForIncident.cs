using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Tagging.Queries
{
    /// <summary>
    ///     Get all tags that the system have identified for an incident.
    /// </summary>
    [Message]
    public class GetTagsForIncident : Query<TagDTO[]>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetTagsForIncident" />.
        /// </summary>
        /// <param name="incidentId">Incident to get tags for</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetTagsForIncident(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Incident to get tags for
        /// </summary>
        public int IncidentId { get; private set; }
    }
}