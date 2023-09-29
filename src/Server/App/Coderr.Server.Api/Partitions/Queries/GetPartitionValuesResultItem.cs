using System;

namespace Coderr.Server.Api.Partitions.Queries
{
    /// <summary>
    /// Item for <see cref="GetPartitionValuesResult"/>.
    /// </summary>
    public class GetPartitionValuesResultItem
    {
        /// <summary>
        /// ID of this entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Most recent date when this value were received.
        /// </summary>
        public DateTime ReceivedAtUtc { get; set; }

    }
}