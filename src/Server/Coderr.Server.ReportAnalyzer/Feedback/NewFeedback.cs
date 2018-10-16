using System;
using System.Collections.Generic;
using System.Text;
using Coderr.Server.Api.Core.Feedback.Commands;

namespace Coderr.Server.ReportAnalyzer.Feedback
{
    public class NewFeedback
    {
         /// <summary>
        ///     Initializes a new instance of the <see cref="NewFeedback" /> class.
        /// </summary>
        /// <param name="errorId">Client side id.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     errorId
        ///     or
        ///     remoteAddress
        /// </exception>
        public NewFeedback(string errorId, string remoteAddress)
        {
            if (errorId == null) throw new ArgumentNullException("errorId");
            if (remoteAddress == null) throw new ArgumentNullException("remoteAddress");
            RemoteAddress = remoteAddress;
            ErrorId = errorId;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NewFeedback" /> class.
        /// </summary>
        /// <param name="reportId">Error report identity.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     remoteAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">reportId</exception>
        public NewFeedback(int reportId, string remoteAddress)
        {
            if (remoteAddress == null) throw new ArgumentNullException("remoteAddress");
            if (reportId <= 0) throw new ArgumentOutOfRangeException("reportId");

            RemoteAddress = remoteAddress;
            ReportId = reportId;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected NewFeedback()
        {
        }

        /// <summary>
        ///     When the feedback was created in the client library
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Email address (user want to get status updates)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Error id generated in our client library. Used to identify error reports before they have been saved into our
        ///     system
        /// </summary>
        public string ErrorId { get; private set; }

        /// <summary>
        ///     Error description
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        ///     IP that the user connected from. either taken from the error report or from the HTTP POST if the UI less client
        ///     library directed the user to our web site.
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        ///     PK from the db entry of the error report.
        /// </summary>
        public int ReportId { get; private set; }
    }
}
