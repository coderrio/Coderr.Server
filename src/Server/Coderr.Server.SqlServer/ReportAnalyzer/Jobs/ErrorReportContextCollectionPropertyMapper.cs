using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Jobs
{
    class ErrorReportContextCollectionPropertyMapper : CrudEntityMapper<ErrorReportContextCollectionProperty>
    {
        public ErrorReportContextCollectionPropertyMapper() : base("ErrorReportCollectionProperties")
        {

        }
    }
}