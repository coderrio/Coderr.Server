namespace Coderr.Server.Api.Partitions.Queries
{
    public class GetPartitionResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PartitionKey { get; set; }
        public int NumberOfItems { get; set; }
        public int Weight { get; set; }
        public int? ImportantThreshold { get; set; }
        public int? CriticalThreshold { get; set; }
    }
}