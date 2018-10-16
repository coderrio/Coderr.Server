using Coderr.Server.App.Core.Reports.Config;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;

namespace Coderr.Server.Web.Infrastructure
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