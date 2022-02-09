using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Server.Domain.Core.ErrorReports;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    public interface IReportFilter
    {
        Task<FilterResult> Filter(FilterContext context);
    }

    public class FilterContext
    {
        public ErrorReportEntity ErrorReport { get; set; }
    }
}
