using System;

namespace codeRR.Server.ReportAnalyzer.LibContracts
{
    /// <summary>
    ///     Failed to identify an incoming report. Stored for debugging purposes.
    /// </summary>
    public class InvalidReport
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvalidReport" />.
        /// </summary>
        /// <param name="appKey">Application key that the client sent</param>
        /// <param name="body">Report exactly as we received it (unpacked)</param>
        /// <param name="exception">Exception thrown when we tried to unpack and store it.</param>
        public InvalidReport(string appKey, byte[] body, Exception exception)
        {
            if (appKey == null) throw new ArgumentNullException("appKey");
            if (body == null) throw new ArgumentNullException("body");

            AppKey = appKey;
            Body = body;
            Exception = exception;
        }

        /// <summary>
        ///     Application key that the client sent
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        ///     Report body
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        ///     Exception thrown when we tried to unpack and store the report.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}