namespace Coderr.Server.Api.Partitions.Commands
{
    [Message]
    public class CreatePartition
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public int NumberOfItems { get; set; }

        public int ImportantThreshold { get; set; }
        public int CriticalThreshold { get; set; }

        public string PartitionKey { get; set; }
        public int Weight { get; set; }
    }
}