namespace Coderr.Server.Api.Partitions.Commands
{
    [Message]
    public class UpdatePartition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfItems { get; set; }
        public int Weight { get; set; }
        public int? ImportantThreshold { get; set; }
        public int? CriticalThreshold { get; set; }
    }
}