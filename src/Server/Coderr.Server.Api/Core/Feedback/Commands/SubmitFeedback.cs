using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Feedback.Commands
{
    /// <summary>
    ///     A user that experienced an error have either followed the link to our website to submit an error or have entered it
    ///     directly into our client library integration.
    /// </summary>
    [Message]
    public class SubmitFeedback
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SubmitFeedback" /> class.
        /// </summary>
        /// <param name="errorId">Client side id.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     errorId
        ///     or
        ///     remoteAddress
        /// </exception>
        public SubmitFeedback(string errorId, string remoteAddress)
        {
            if (errorId == null) throw new ArgumentNullException("errorId");
            if (remoteAddress == null) throw new ArgumentNullException("remoteAddress");
            RemoteAddress = remoteAddress;
            ErrorId = errorId;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubmitFeedback" /> class.
        /// </summary>
        /// <param name="reportId">Error report identity.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     remoteAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">reportId</exception>
        public SubmitFeedback(int reportId, string remoteAddress)
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
        protected SubmitFeedback()
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
        [Required]
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

        /// <summary>
        ///     Validate contents of this command
        /// </summary>
        /// <param name="validationContext">validation context</param>
        /// <returns>Validation errors if any</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext == null) throw new ArgumentNullException("validationContext");

            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Feedback))
                return new[] {new ValidationResult("Email or Feedback must be given")};
            return new ValidationResult[0];
        }
    }
}