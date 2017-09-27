using System.Collections.Generic;
using Griffin.Data.Mapper;
using codeRR.App.Modules.Triggers.Domain;
using codeRR.SqlServer.Tools;

namespace codeRR.SqlServer.Modules.Triggers
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