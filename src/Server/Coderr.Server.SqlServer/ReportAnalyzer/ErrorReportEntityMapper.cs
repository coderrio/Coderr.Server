using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer
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

            Property(x => x.ContextCollections)
                .ColumnName("ContextInfo")
                .ToPropertyValue(x => null)
                .ToColumnValue(x => "");

            Property(x => x.ClientReportId)
                .ColumnName("ErrorId");

            Property(x => x.User)
                .Ignore();

        }
    }
}