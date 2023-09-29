namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// Result for <see cref="GetIterations"/>.
    /// </summary>
    public class GetIterationsResult
    {
        /// <summary>
        /// Found paths.
        /// </summary>
        public GetIterationsResultItem[] Items { get; set; }
    }
}