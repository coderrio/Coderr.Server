using Coderr.Server.Domain.Modules.Partitions;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    public class IncidentPartitionValuesMapper : CrudEntityMapper<IncidentPartitionValue>
    {
        public IncidentPartitionValuesMapper() : base("IncidentPartitionValues")
        {
            Property(x => x.Id).PrimaryKey(true);
            Property(x => x.IncidentId);
            Property(x => x.PartitionId);
            Property(x => x.ValueId);
        }
    }
}