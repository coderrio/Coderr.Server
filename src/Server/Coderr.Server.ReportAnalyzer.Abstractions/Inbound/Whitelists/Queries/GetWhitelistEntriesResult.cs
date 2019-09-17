namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Whitelists.Queries
{
    /// <summary>
    /// Result for <see cref="GetWhitelistEntries"/>.
    /// </summary>
    public class GetWhitelistEntriesResult
    {
        public GetWhitelistEntriesResultItem[] Entries { get; set; }
    }
}