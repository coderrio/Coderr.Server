using System;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.Inbound.Handlers.Reports
{
    public interface IReportAnalyzer
    {
        /// <summary>
        ///     Analyze report
        /// </summary>
        /// <param name="report">report</param>
        /// <exception cref="ArgumentNullException">report</exception>
        Task Analyze(IMessageContext context, ErrorReportEntity report);
    }
}