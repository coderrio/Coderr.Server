using DotNetCqs;

namespace Coderr.Server.Api.Modules.Whitelists.Queries
{
    /// <summary>
    /// Get whitelist either by application id or DomainName
    /// </summary>
    [Message]
    public class GetWhitelistEntries : Query<GetWhitelistEntriesResult>
    {
        /// <summary>
        /// Limit result to this application only
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Limit result to this domain name only
        /// </summary>
        public string DomainName { get; set; }
    }
}
