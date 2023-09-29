using Coderr.Server.Api.Partitions.Queries;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    internal class GetPartitionValuesResultItemMapper : EntityMapper<GetPartitionValuesResultItem>
    {
        public GetPartitionValuesResultItemMapper()
        {
            Property(x => x.Id)
                .PrimaryKey(true);
        }
    }
}