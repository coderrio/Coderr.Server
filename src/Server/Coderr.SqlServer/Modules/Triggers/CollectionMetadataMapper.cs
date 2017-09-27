using System.Collections.Generic;
using codeRR.Server.App.Modules.Triggers.Domain;
using codeRR.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Triggers
{
    public class CollectionMetadataMapper : CrudEntityMapper<CollectionMetadata>
    {
        public CollectionMetadataMapper() : base("CollectionMetaData")
        {
            Property(x => x.IsUpdated).Ignore();

            Property(x => x.Properties)
                .ToPropertyValue(EntitySerializer.Deserialize<ICollection<string>>);
        }
    }
}