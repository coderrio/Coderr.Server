using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Analysis.Jobs
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
