using System;
using System.Collections.Generic;
using System.Text;
using Griffin.Data.Mapper;

namespace Coderr.Server.ReportAnalyzer.Inbound.Whitelist
{
    class WhitelistedDomainMapper : CrudEntityMapper<WhitelistedDomain>
    {
        public WhitelistedDomainMapper() : base("ReportingWhitelistedDomains")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

        }
    }
}
