using System;
using System.Collections.Generic;
using System.Text;
using Coderr.Server.Domain.Modules.History;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.History
{
    class HistoryEntryMapper : CrudEntityMapper<HistoryEntry>
    {
        public HistoryEntryMapper() : base("IncidentHistory")
        {
            Property(x => x.Id).PrimaryKey(true);
            Property(x => x.IncidentState)
                .ColumnName("State");
            Property(x => x.AccountId)
                .ToColumnValue(x => (object)x ?? DBNull.Value)
                .ToPropertyValue(x => (int?)(x is DBNull ? null : x));
        }
    }
}
