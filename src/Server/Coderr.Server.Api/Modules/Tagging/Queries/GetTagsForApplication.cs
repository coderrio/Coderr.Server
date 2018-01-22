using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Tagging.Queries
{
    /// <summary>
    ///     Get all tags that the system have identified for an incident.
    /// </summary>
    [Message]
    public class GetTagsForApplication : Query<TagDTO[]>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetTagsForIncident" />.
        /// </summary>
        /// <param name="applicationId">Incident to get tags for</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetTagsForApplication(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Incident to get tags for
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}