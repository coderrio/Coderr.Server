using System.Collections;
using System.Collections.Generic;
using Griffin.Data.Mapper;
using Newtonsoft.Json;
using OneTrueError.App.Modules.Triggers.Domain;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Modules.Triggers
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