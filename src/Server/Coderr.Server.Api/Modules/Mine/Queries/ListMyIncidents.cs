using DotNetCqs;

namespace Coderr.Server.Api.Modules.Mine.Queries
{
    /// <summary>
    /// Get the users assigned incidents and the ones that are recommended for that person.
    /// </summary>
    [Message]
    public class ListMyIncidents : Query<ListMyIncidentsResult>
    {
        /// <summary>
        /// Limit to the given application (if specified).
        /// </summary>
        public int? ApplicationId { get; set; }
    }
}
