using System;

namespace OneTrueError.ReportAnalyzer.LibContracts
{
    /// <summary>
    ///     DTO for a client library report. Should always look exactly as the report in the OneTrueError.Client assembly
    /// </summary>
    [Serializable]
    public class ReceivedReportDTO
    {
        /// <summary>
        ///     Application that this report belongs to
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     A collection of context information such as HTTP request information or computer hardware info.
        /// </summary>
        public ReceivedReportContextInfo[] ContextCollections { get; set; }

        /// <summary>
        ///     Date specified at client side
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     When the report receiver stored this report
        /// </summary>
        public DateTime DateReceivedUtc { get; set; }


        /// <summary>
        ///     Exception which was caught.
        /// </summary>
        public ReceivedReportException Exception { get; set; }

        /// <summary>
        ///     Remote address that we received the report from
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        ///     Gets incident id (unique identifier used in communication with the customer to identify this error)
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        ///     Version of the report (version of the OneTrueError.Reporting API contract)
        /// </summary>
        public string ReportVersion { get; set; }
    }
}