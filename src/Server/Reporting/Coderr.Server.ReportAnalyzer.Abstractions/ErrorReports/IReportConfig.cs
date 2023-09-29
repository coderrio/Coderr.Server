using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports
{
    public interface IReportConfig
    {
        int MaxReportJsonSize { get; }
    }
}
