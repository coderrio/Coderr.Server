using Coderr.Server.Abstractions.Reports;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;

namespace Coderr.Server.WebSite.Infrastructure.Adapters
{
    public class ReportConfigWrapper : IReportConfig
    {
        private readonly ReportConfig _inner;

        public ReportConfigWrapper(ReportConfig inner)
        {
            _inner = inner;
        }

        public int MaxReportJsonSize => _inner.MaxReportJsonSize;
    }
}