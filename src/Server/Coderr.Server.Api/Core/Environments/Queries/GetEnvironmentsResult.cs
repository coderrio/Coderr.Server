namespace Coderr.Server.Api.Core.Environments.Queries
{
    /// <summary>
    ///     Result for <see cref="GetEnvironments" />
    /// </summary>
    public class GetEnvironmentsResult
    {
        /// <summary>
        ///     Get name of each environment
        /// </summary>
        public GetEnvironmentsResultItem[] Items { get; set; }
    }
}