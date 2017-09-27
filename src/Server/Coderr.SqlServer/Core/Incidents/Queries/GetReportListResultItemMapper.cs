using Griffin.Data.Mapper;
using codeRR.Api.Core.Reports.Queries;

namespace codeRR.SqlServer.Core.Incidents.Queries
{
    public class GetReportListResultItemMapper : EntityMapper<GetReportListResultItem>
    {
        public GetReportListResultItemMapper()
        {
            Property(x => x.Message).ColumnName("Title");
        }
    }
}