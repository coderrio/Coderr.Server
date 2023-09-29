namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// Result for <see cref="GetProjects"/>
    /// </summary>
    public class GetProjectsResult
    {
        /// <summary>
        /// Found paths.
        /// </summary>
        public GetProjectsResultItem[] Items { get; set; }
    }
}