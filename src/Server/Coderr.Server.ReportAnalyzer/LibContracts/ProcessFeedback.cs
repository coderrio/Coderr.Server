using System;

namespace codeRR.Server.ReportAnalyzer.LibContracts
{
    /// <summary>
    ///     Feedback item as received by the client library
    /// </summary>
    [Serializable]
    public class ProcessFeedback
    {
        /// <summary>
        ///     Application that the report belongs to.
        /// </summary>
        /// <remarks>
        ///     Added when the application is identified in the server.
        /// </remarks>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     What the user typed about what he did when the exception occurred.
        /// </summary>
        /// <remarks>
        ///     <para>From the library contract</para>
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        ///     User want to get status updates.
        /// </summary>
        /// <remarks>
        ///     <para>From the library contract</para>
        /// </remarks>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     When the report receiver stored this report
        /// </summary>
        public DateTime ReceivedAtUtc { get; set; }

        /// <summary>
        ///     Remote address that we received the report from
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        ///     Unique id for this report.
        /// </summary>
        /// <remarks>
        ///     <para>From the library contract</para>
        /// </remarks>
        public string ReportId { get; set; }

        /// <summary>
        ///     Version of the report (version of the codeRR.Reporting API contract)
        /// </summary>
        public string ReportVersion { get; set; }
    }
}