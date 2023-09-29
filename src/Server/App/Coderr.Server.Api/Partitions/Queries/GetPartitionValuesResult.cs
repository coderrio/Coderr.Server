namespace Coderr.Server.Api.Partitions.Queries
{
    /// <summary>
    /// Result for <see cref="GetPartitionValues"/>.
    /// </summary>
    public class GetPartitionValuesResult
    {
        /// <summary>
        /// All matching items.
        /// </summary>
        public GetPartitionValuesResultItem[] Items { get; set; }
    }
}