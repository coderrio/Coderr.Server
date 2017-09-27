using System;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Core.Reports.Invalid
{
    /// <summary>
    ///     Invalid report are reports that we have received from the client library but failed to deserialize or identify.
    /// </summary>
    public class InvalidErrorReport
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvalidErrorReport" />.
        /// </summary>
        /// <param name="applicationId">primary key</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public InvalidErrorReport(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            AddedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     When report was uploaded
        /// </summary>
        public DateTime AddedAtUtc { get; private set; }

        /// <summary>
        ///     Application id (identified with the help of the AppKey)
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Exception message telling why we couldn't serialize/authenticate it.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        ///     Report id
        /// </summary>
        public int Id { get; private set; }


        /// <summary>
        ///     Report, as uploaded by the client
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "I like my arrays.")]
        public byte[] Report { get; set; }
    }
}