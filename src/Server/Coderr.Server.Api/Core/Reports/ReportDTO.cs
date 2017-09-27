using System;

namespace codeRR.Server.Api.Core.Reports
{
    /// <summary>
    ///     Report representation.
    /// </summary>
    public class ReportDTO
    {
        /// <summary>
        ///     Application that the incident and report belongs in.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     A collection of context information such as HTTP request information or computer hardware info.
        /// </summary>
        public ContextCollectionDTO[] ContextCollections { get; set; }

        /// <summary>
        ///     Date specified at client side
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Exception which was caught.
        /// </summary>
        public ReportExeptionDTO Exception { get; set; }

        /// <summary>
        ///     DB primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     DB primary key
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Ip of the report uploader.
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        ///     Gets error id (unique identifier used in communication with the customer to identify this error)
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        ///     Version of the report
        /// </summary>
        public string ReportVersion { get; set; }
    }
}