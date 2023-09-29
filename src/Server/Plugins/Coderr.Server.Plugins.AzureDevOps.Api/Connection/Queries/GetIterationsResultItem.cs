namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// Item in <see cref="GetIterationsResult"/>
    /// </summary>
    public class GetIterationsResultItem
    {
        /// <summary>
        /// Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path to use in work items
        /// </summary>
        public string Path { get; set; }
    }
}