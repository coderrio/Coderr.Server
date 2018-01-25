using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Analysis.Jobs
{
    class ErrorReportContextCollectionPropertyMapper : CrudEntityMapper<ErrorReportContextCollectionProperty>
    {
        public ErrorReportContextCollectionPropertyMapper() : base("ErrorReportCollectionProperties")
        {

        }
    }
}