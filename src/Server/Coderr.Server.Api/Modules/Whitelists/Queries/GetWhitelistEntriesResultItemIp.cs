using System;

namespace Coderr.Server.Api.Modules.Whitelists.Queries
{
    public class GetWhitelistEntriesResultItemIp
    {
        public string Address { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public ResultItemIpType Type { get; set; }
    }
}