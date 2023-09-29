using System;
using System.Collections.Generic;
using Coderr.Server.Abstractions.WorkItems;
using Griffin.Data.Mapper;
using Newtonsoft.Json;

namespace Coderr.Server.SqlServer.WorkItems
{
    class WorkItemUserMappingMap : CrudEntityMapper<WorkItemUserMapping>
    {
        public WorkItemUserMappingMap() : base("WorkItemUserMappings")
        {
            Property(x => x.AccountId).PrimaryKey(false);
            Property(x => x.AdditionalData)
                .ToColumnValue2(x => x.Value == null ? null : JsonConvert.SerializeObject(x.Value))
                .ToPropertyValue2(x =>
                    x.Value is DBNull
                        ? null
                        : JsonConvert.DeserializeObject<IDictionary<string, string>>((string)x.Value));
        }
    }
}
