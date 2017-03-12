using DotNetCqs;

namespace OneTrueError.App.Modules.Versions.Events
{
    /// <summary>
    ///     We received a report for a new version
    /// </summary>
    public class NewVersionReported : ApplicationEvent
    {
        /// <summary>
        ///     Which application the version was reported for
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Version that we got a report for
        /// </summary>
        /// <remarks>
        ///     Formatted as a typical assembly version
        /// </remarks>
        public string Version { get; set; }
    }
}