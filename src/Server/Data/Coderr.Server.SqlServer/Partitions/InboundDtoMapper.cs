using Coderr.Server.ReportAnalyzer.Partitions;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    public class InboundDtoMapper : CrudEntityMapper<InboundDTO>
    {
        public InboundDtoMapper() : base("InboundPartitionValues")
        {
            
        }
    }
}