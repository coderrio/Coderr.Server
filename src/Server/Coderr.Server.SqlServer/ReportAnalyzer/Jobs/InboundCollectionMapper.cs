using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Jobs
{
    class InboundCollectionMapper : CrudEntityMapper<InboundCollection>
    {
        public InboundCollectionMapper() : base("ErrorReportCollectionInbound")
        {
            Property(x => x.JsonData)
                .ColumnName("Body");

        }
    }
}
