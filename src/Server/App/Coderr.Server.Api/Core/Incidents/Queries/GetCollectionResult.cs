namespace Coderr.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    /// Result for <see cref="GetCollection"/>.
    /// </summary>
    public class GetCollectionResult
    {
        /// <summary>
        /// Fetched collections.
        /// </summary>
        public GetCollectionResultItem[] Items { get; set; }
    }
}