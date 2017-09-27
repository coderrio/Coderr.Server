using System;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Core.Feedback
{
    /// <summary>
    ///     We've failed to save this report.
    /// </summary>
    public class InvalidErrorReport
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvalidErrorReport" />.
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public InvalidErrorReport(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            AddedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     When this report was uploaded.
        /// </summary>
        public DateTime AddedAtUtc { get; private set; }

        /// <summary>
        ///     Application id
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Why the report receiving failed.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        ///     Invalid report id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Raw report bytes (i.e. not deserialized)
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "I like my arrays.")]
        public byte[] Report { get; set; }
    }
}