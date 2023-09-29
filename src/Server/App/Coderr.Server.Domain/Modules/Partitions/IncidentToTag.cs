namespace Coderr.Server.Domain.Modules.Partitions
{
    public class IncidentToTag
    {
        public int IncidentId { get; set; }
        public int Count { get; set; }
        public int PartitionId { get; set; }

        public bool MarkAsImportant { get; set; }
        public bool MarkAsCritical { get; set; }
        public int ApplicationId { get; set; }
    }
}