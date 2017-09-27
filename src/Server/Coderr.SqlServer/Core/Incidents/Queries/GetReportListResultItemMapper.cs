using codeRR.Server.Api.Core.Reports.Queries;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    public class GetReportListResultItemMapper : EntityMapper<GetReportListResultItem>
    {
        public GetReportListResultItemMapper()
        {
            Property(x => x.Message).ColumnName("Title");
        }
    }
}