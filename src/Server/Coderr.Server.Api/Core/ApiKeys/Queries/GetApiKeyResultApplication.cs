namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     An allowed application for <see cref="GetApiKeyResult" />.
    /// </summary>
    public class GetApiKeyResultApplication
    {
        /// <summary>
        ///     Application id (PK)
        /// </summary>
        public int ApplicationId { get; set; }


        /// <summary>
        ///     Name of the application
        /// </summary>
        public string ApplicationName { get; set; }
    }
}