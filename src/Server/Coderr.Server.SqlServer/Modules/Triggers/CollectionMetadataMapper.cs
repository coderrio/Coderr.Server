using System.Collections.Generic;
using Coderr.Server.ReportAnalyzer.Triggers;
using Coderr.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Triggers
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