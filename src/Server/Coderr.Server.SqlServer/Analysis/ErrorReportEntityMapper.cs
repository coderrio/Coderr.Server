using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Analysis
{
    public class ErrorReportEntityMapper : CrudEntityMapper<ErrorReportEntity>
    {
        public ErrorReportEntityMapper() : base("ErrorReports")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

            Property(x => x.Exception)
                .ToPropertyValue(EntitySerializer.Deserialize<ErrorReportException>)
                .ToColumnValue(EntitySerializer.Serialize);

            Property(x => x.ContextInfo)
                .ToPropertyValue(x => null)
                .ToColumnValue(x => "");

            Property(x => x.ClientReportId)
                .ColumnName("ErrorId");

            Property(x => x.User)
                .Ignore();

        }
    }
}