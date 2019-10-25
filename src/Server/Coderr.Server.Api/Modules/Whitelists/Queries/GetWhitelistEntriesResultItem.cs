namespace Coderr.Server.Api.Modules.Whitelists.Queries
{
    /// <summary>
    /// Entry for <see cref="GetWhitelistEntriesResult"/>
    /// </summary>
    public class GetWhitelistEntriesResultItem
    {
        public int Id { get; set; }
        public GetWhitelistEntriesResultItemIp[] IpAddresses { get; set; }
        public GetWhitelistEntriesResultItemApp[] Applications { get; set; }
        public string DomainName { get; set; }
    }
}