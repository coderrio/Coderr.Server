using System;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Item for <see cref="GetReportResult" />.
    /// </summary>
    public class GetReportListResultItem
    {
        /// <summary>
        ///     When the report was created in the client library
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Report id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Exception message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     IP that uploaded the report.
        /// </summary>
        public string RemoteAddress { get; set; }
    }
}