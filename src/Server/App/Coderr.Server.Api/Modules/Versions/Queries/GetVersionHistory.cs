using System;
using System.Collections.Generic;
using System.Text;
using DotNetCqs;

namespace Coderr.Server.Api.Modules.Versions.Queries
{
    [Message]
    public class GetVersionHistory : Query<GetVersionHistoryResult>
    {
        public int ApplicationId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
