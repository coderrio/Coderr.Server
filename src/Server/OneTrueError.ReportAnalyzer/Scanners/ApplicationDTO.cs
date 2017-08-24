namespace OneTrueError.ReportAnalyzer.Scanners
{
    /// <summary>
    ///     Application
    /// </summary>
    public class ApplicationDTO
    {
        /// <summary>
        ///     Identity
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Application key
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        ///     Shared key (used to sign the uploaded reports)
        /// </summary>
        public string SharedSecret { get; set; }
    }
}