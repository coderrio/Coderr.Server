namespace OneTrueError.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationInfo" />.
    /// </summary>
    public class GetApplicationInfoResult
    {
        /// <summary>
        ///     App key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        ///     Type of application
        /// </summary>
        public TypeOfApplication ApplicationType { get; set; }

        /// <summary>
        ///     Application id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name of the application.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Shared secret, used together with <see cref="AppKey" /> to make sure that the reports come from the correct source.
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        ///     Total nume rof incidents for this application.
        /// </summary>
        public int TotalIncidentCount { get; set; }
    }
}