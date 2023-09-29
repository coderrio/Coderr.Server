using Coderr.Server.Api.Core.Reports.Queries;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
{
    public class GetReportListResultItemMapper : EntityMapper<GetReportListResultItem>
    {
        public GetReportListResultItemMapper()
        {
            Property(x => x.Message).ColumnName("Title");
        }
    }
}