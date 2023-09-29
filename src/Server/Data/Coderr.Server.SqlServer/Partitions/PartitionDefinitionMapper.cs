using System;
using Coderr.Server.Domain.Modules.Partitions;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    public class PartitionDefinitionMapper : CrudEntityMapper<PartitionDefinition>
    {
        public PartitionDefinitionMapper() : base("PartitionDefinitions")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.NumberOfItems)
                .ToColumnValue(x => x == 0 ? (object) DBNull.Value : x)
                .ToPropertyValue(x => (int) (x == DBNull.Value ? 0 : x));
        }
    }
}