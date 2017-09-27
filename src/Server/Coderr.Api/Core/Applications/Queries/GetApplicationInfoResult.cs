namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationInfo" />.
    /// </summary>
    public class GetApplicationInfoResult
    {
        /// <summary>
        ///     Application key
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
        ///     Total number of incidents for this application.
        /// </summary>
        public int TotalIncidentCount { get; set; }

        /// <summary>
        /// Versions that we have received error reports for.
        /// </summary>
        public string[] Versions { get; set; }
    }
}