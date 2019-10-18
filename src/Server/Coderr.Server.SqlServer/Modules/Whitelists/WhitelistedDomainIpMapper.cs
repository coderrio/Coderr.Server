using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Coderr.Server.App.Modules.Whitelists;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    class WhitelistedDomainIpMapper : CrudEntityMapper<WhitelistedDomainIp>
    {
        public WhitelistedDomainIpMapper() : base("WhitelistedDomainIps")
        {
            Property(x => x.IpAddress)
                .ToColumnValue(x => x.ToString())
                .ToPropertyValue(x => IPAddress.Parse((string)x));
            Property(x => x.IpType)
                .ToColumnValue(x => (int) x)
                .ToPropertyValue(x => (IpType) x);
        }
    }
}
