namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Context data that can help the developer to directly understand why the exception happened.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For instance for Page Not Found this is the URL and the Referrer.
    ///     </para>
    /// </remarks>
    public class HighlightedContextData
    {
        /// <summary>
        ///     Why this data helps and what it means.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Name ("UrlReferrer", "Url", "HttpCode" etc).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Optional url that the user can click on to get more information
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Value to show
        /// </summary>
        /// <remarks>
        ///     Values should be sorted i priority order (first item will be displayed directly)
        /// </remarks>
        public string[] Value { get; set; }
    }
}