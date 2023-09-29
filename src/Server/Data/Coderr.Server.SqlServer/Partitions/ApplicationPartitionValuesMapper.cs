using Coderr.Server.Domain.Modules.Partitions;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    public class ApplicationPartitionValuesMapper : CrudEntityMapper<ApplicationPartitionValue>
    {
        public ApplicationPartitionValuesMapper() : base("ApplicationPartitionValues")
        {
            Property(x => x.Id).PrimaryKey(true);
            Property(x => x.PartitionId);
            Property(x => x.Value);
        }
    }
}