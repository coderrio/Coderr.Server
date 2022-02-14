using Coderr.Server.Api.Modules.Mine.Queries;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Mine
{
    internal class ListMyIncidentsResultItemMapper : EntityMapper<ListMyIncidentsResultItem>
    {
        public ListMyIncidentsResultItemMapper()
        {
            Property(x => x.DaysOld).Ignore();
        }
    }
}