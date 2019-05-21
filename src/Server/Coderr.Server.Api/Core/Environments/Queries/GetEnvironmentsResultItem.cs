namespace Coderr.Server.Api.Core.Environments.Queries
{
    /// <summary>
    /// Item for <see cref="GetEnvironmentsResult"/>.
    /// </summary>
    public class GetEnvironmentsResultItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name, like "Production" or "Test"
        /// </summary>
        public string Name { get; set; }
    }
}