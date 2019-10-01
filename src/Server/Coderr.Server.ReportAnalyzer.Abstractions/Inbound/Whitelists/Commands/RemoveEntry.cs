namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Whitelists.Commands
{
    /// <summary>
    ///     Remove a previously added white list entry
    /// </summary>
    public class RemoveEntry
    {
        /// <summary>
        ///     Id of the entry.
        /// </summary>
        public int Id { get; set; }
    }
}