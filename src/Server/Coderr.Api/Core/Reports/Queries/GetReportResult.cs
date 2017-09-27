using System;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Result for <see cref="GetReportResult" />.
    /// </summary>
    public class GetReportResult
    {
        /// <summary>
        ///     Context collections
        /// </summary>
        public GetReportResultContextCollection[] ContextCollections { get; set; }

        /// <summary>
        ///     When the report was created in the client library
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Email address (if the user would like to get status updates).
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Unique id generated in the client library
        /// </summary>
        public string ErrorId { get; set; }

        /// <summary>
        ///     Actual exception
        /// </summary>
        public GetReportException Exception { get; set; }

        /// <summary>
        ///     Report id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Incident that this report belongs to.
        /// </summary>
        public string IncidentId { get; set; }

        /// <summary>
        ///     First line from the exception message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Stack trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     Error description written by the user (if any).
        /// </summary>
        public string UserFeedback { get; set; }
    }
}