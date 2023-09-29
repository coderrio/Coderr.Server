namespace Coderr.Server.Domain.Modules.Partitions
{
    /// <summary>
    /// </summary>
    public class IncidentPartitionSummary
    {
        /// <summary>
        ///     Partition id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Incident
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Title of partition
        /// </summary>
        public string PartitionTitle { get; set; }

        /// <summary>
        ///     Calculated severity (<see cref="PartitionDefinition.CalculateSeverity" />)
        /// </summary>
        /// <value>
        ///     Largest value is most severe.
        /// </value>
        public int Severity { get; set; }


        /// <summary>
        ///     Total number of possible values
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Either NumberOfItems from <see cref="PartitionDefinition" /> or the total amount of unique values recieved for
        ///         the
        ///         partition.
        ///     </para>
        /// </remarks>
        public int TotalCount { get; set; }

        /// <summary>
        /// Number of items that must be affected for this incident to be considered important.
        /// </summary>
        public int ImportantThreshold { get; set; }

        /// <summary>
        /// Number of items that must be affected for this incident to be considered critical.
        /// </summary>
        public int CriticalThreshold { get; set; }

        /// <summary>
        ///     Number of unique values that have been received for this incident
        /// </summary>
        public int ValueCount { get; set; }
    }
}