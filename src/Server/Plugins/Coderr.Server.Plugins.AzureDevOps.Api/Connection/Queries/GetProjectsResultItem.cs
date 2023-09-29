namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// Item in <see cref="GetProjectsResult"/>
    /// </summary>
    public class GetProjectsResultItem
    {
        /// <summary>
        /// Identity
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name 
        /// </summary>
        public string Name { get; set; }
    }
}