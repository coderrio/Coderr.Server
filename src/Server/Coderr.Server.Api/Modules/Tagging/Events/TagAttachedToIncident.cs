using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Tagging.Events
{
    /// <summary>
    ///     New tag(s) have been identified for the processed incident.
    /// </summary>
    [Message]
    public class TagAttachedToIncident
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TagAttachedToIncident" />.
        /// </summary>
        /// <param name="incidentId">Incident being processed</param>
        /// <param name="tags">tags</param>
        /// <exception cref="ArgumentNullException">tags</exception>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public TagAttachedToIncident(int incidentId, TagDTO[] tags)
        {
            if (tags == null) throw new ArgumentNullException("tags");
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            Tags = tags;
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Incident being processed
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Identified tags
        /// </summary>
        public TagDTO[] Tags { get; private set; }
    }
}