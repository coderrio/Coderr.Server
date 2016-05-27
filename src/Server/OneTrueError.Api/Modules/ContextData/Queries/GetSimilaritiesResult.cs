namespace OneTrueError.Api.Modules.ContextData.Queries
{
    /// <summary>
    ///     Result for <see cref="GetSimilarities" />.
    /// </summary>
    public class GetSimilaritiesResult
    {
        /// <summary>
        ///     All analyzed context collections
        /// </summary>
        public GetSimilaritiesCollection[] Collections { get; set; }
    }
}