using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.ReportAnalyzer.Jobs
{
    class ErrorReportContextCollectionPropertyMapper : CrudEntityMapper<ErrorReportContextCollectionProperty>
    {
        public ErrorReportContextCollectionPropertyMapper() : base("ErrorReportCollectionProperties")
        {

        }
    }
}