using Coderr.Server.Api.Modules.Mine.Queries;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Mine
{
    internal class ListMySuggestedItemMapper : EntityMapper<ListMySuggestedItem>
    {
        public ListMySuggestedItemMapper()
        {
            Property(x => x.ExceptionTypeName).ColumnName("FullName");
        }
    }
}