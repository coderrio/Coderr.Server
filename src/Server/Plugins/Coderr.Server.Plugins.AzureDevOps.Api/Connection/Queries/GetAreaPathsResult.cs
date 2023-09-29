namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// Result for <see cref="GetAreaPaths"/>
    /// </summary>
    public class GetAreaPathsResult
    {
        /// <summary>
        /// Found paths.
        /// </summary>
        public GetAreaPathsResultItem[] Items { get; set; }
    }
}