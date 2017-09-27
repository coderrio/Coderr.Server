using System;

namespace codeRR.Server.App.Modules.Versions
{
    /// <summary>
    ///     An version that we track.
    /// </summary>
    public class ApplicationVersion
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationVersion" />.
        /// </summary>
        /// <param name="applicationId">FK</param>
        /// <param name="applicationName">Name of the application</param>
        /// <param name="version">Version (x.x.x.x)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ApplicationVersion(int applicationId, string applicationName, string version)
        {
            if (applicationName == null) throw new ArgumentNullException("applicationName");
            if (version == null) throw new ArgumentNullException("version");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            ApplicationName = applicationName;
            Version = version;
            ReceivedFirstReportAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected ApplicationVersion()
        {
            
        }

        /// <summary>
        ///     Id of the application that this version is for
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Name of the application that this version is for
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        ///     ID of this entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     When we received the first report for this version
        /// </summary>
        public DateTime ReceivedFirstReportAtUtc { get; private set; }

        /// <summary>
        ///     When we received the last report for this version
        /// </summary>
        public DateTime ReceivedLastReportAtUtc { get; private set; }

        /// <summary>
        ///     Assembly version (x.x.x.x)
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        ///     Update the date that tracks the last time we received a report
        /// </summary>
        public void UpdateReportDate()
        {
            ReceivedLastReportAtUtc = DateTime.UtcNow;
        }
    }
}