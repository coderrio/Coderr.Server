using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Reports.Queries;

namespace OneTrueError.SqlServer.Core.Incidents.Queries
{
    public class GetReportListResultItemMapper : EntityMapper<GetReportListResultItem>
    {
        public GetReportListResultItemMapper()
        {
            Property(x => x.Message).ColumnName("Title");
        }
    }
}