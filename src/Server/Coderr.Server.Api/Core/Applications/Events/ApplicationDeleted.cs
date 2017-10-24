using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Events
{
    /// <summary>
    ///     An application have been deleted.
    /// </summary>
    [Message]
    public class ApplicationDeleted
    {
        /// <summary>
        ///     Key used when uploading reports
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        ///     Database PK
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application
        /// </summary>
        public string ApplicationName { get; set; }
    }
}