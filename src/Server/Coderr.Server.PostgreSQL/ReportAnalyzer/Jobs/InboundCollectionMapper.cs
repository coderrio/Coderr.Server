using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.ReportAnalyzer.Jobs
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
