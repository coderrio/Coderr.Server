namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     Item for <see cref="ListApiKeysResult" />.
    /// </summary>
    public class ListApiKeysResultItem
    {
        /// <summary>
        ///     Key to use
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Application name, i.e. name of the application that uses this key.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Identity
        /// </summary>
        public int Id { get; set; }
    }
}