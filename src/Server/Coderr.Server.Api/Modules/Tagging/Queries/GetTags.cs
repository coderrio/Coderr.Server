using DotNetCqs;

namespace codeRR.Server.Api.Modules.Tagging.Queries
{
    /// <summary>
    ///     Get all tags that the system have identified for an incident.
    /// </summary>
    [Message]
    public class GetTags : Query<TagDTO[]>
    {
        /// <summary>
        ///     Application to get tags for
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        ///     Incident to get tags for
        /// </summary>
        public int? IncidentId { get; private set; }
    }
}