namespace Coderr.Server.Api.Web.Overview.Queries
{
    /// <summary>
    /// Summary for a partition
    /// </summary>
    public class PartitionOverview
    {
        /// <summary>
        /// Name, used when reporting errors.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name to show for users.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Amount of unique values for this partition.
        /// </summary>
        public int Value { get; set; }
    }
}