namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     Result for <see cref="ListApiKeys" />.
    /// </summary>
    public class ListApiKeysResult
    {
        /// <summary>
        ///     All created keys
        /// </summary>
        public ListApiKeysResultItem[] Keys { get; set; }
    }
}