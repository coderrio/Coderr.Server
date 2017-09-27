using System;

namespace codeRR.Server.ReportAnalyzer.Inbound.Models
{
    /// <summary>
    ///     Report as uploaded by the client API.
    /// </summary>
    public class NewReportDTO
    {
        /// <summary>
        ///     A collection of context information such as HTTP request information or computer hardware info.
        /// </summary>
        public NewReportContextInfo[] ContextCollections { get; set; }

        /// <summary>
        ///     Date specified at client side
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }


        /// <summary>
        ///     Exception which was caught.
        /// </summary>
        public NewReportException Exception { get; set; }

        public string RemoteAddress { get; set; }

        /// <summary>
        ///     Gets incident id (unique identifier used in communication with the customer to identify this error)
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        ///     Version of the report
        /// </summary>
        public string ReportVersion { get; set; }
    }
}