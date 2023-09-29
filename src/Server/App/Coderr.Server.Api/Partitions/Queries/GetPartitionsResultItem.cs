namespace Coderr.Server.Api.Partitions.Queries
{
    public class GetPartitionsResultItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public string PartitionKey { get; set; }

    }
}