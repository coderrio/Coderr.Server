using DotNetCqs;

namespace Coderr.Server.Api.Partitions.Queries
{
    [Message]
    public class GetPartition : Query<GetPartitionResult>
    {
        public int Id { get; set; }
    }
}