using System;
using System.Collections.Generic;

namespace Coderr.Server.Api.Modules.Versions.Queries
{
    public class GetVersionHistoryResultMonth
    {
        public DateTime YearMonth { get; set; }
        public IList<GetVersionHistoryResultItem> Items { get; set; } = new List<GetVersionHistoryResultItem>();
    }
}