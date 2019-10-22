using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coderr.Server.Domain.Modules.ReportSpikes;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.ReportSpikes
{
    public class ErrorReportSpikeMapper: CrudEntityMapper<ErrorReportSpike>

    {
        public ErrorReportSpikeMapper() : base("ErrorReportSpikes")
        {
            Property(x => x.NotifiedAccounts)
                .ToColumnValue(x => string.Join(",", x))
                .ToPropertyValue(x => ((string) x)
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray()
                );
        }
    }
}
