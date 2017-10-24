using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Get report (i.e. exception and context collections)
    /// </summary>
    [Message]
    public class GetReport : Query<GetReportResult>
    {
        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetReport()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetReport" />.
        /// </summary>
        /// <param name="reportId">report</param>
        /// <exception cref="ArgumentOutOfRangeException">reportId</exception>
        public GetReport(int reportId)
        {
            if (reportId <= 0) throw new ArgumentOutOfRangeException("reportId");
            ReportId = reportId;
        }

        /// <summary>
        ///     Report id
        /// </summary>
        public int ReportId { get; private set; }
    }
}