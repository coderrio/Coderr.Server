using System;

namespace Coderr.Server.ReportAnalyzer.Partitions
{
    /// <summary>
    /// Value received from the client library
    /// </summary>
    public class InboundDTO
    {
        public const string ContextCollectionName = "Partition";
        public const string ContextCollectionPropertyPrefix = "ErrPartition";

        public int IncidentId { get; set; }
        public string Value { get; set; }
        public string PartitionKey { get; set; }
        public DateTime ReceivedAtUtc { get; set; }
    }
}
