using System;

namespace Coderr.Server.Domain.Modules.Partitions
{
    /// <summary>
    ///     Defines a partition.
    /// </summary>
    public class PartitionDefinition
    {
        private int _weight = 1;

        public PartitionDefinition(int applicationId, string title, string partitionKey)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
            Name = title ?? throw new ArgumentNullException(nameof(title));
            PartitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
        }

        protected PartitionDefinition()
        {
        }

        /// <summary>
        ///     Which application this partition is for.
        /// </summary>
        public int ApplicationId { get; private set; }

        public int Id { get; set; }

        /// <summary>
        ///     Name of the partition (human friendly name)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The amount of items that we can receive reports for in this partition.
        /// </summary>
        /// <remarks>
        ///     For instance if the partition is called "UserName", then this value would indicate the total number of users. Allows
        ///     us to
        ///     calculate the percentage of the partition that an incident affects.
        /// </remarks>
        public int NumberOfItems { get; set; }

        /// <summary>
        /// Mark incident as important once the specified amount of items have been affected.
        /// </summary>
        public int? ImportantThreshold { get; set; }

        /// <summary>
        /// Mark incident as critical once the specified amount of items have been affected.
        /// </summary>
        public int? CriticalThreshold { get; set; }

        /// <summary>
        ///     Name of the item that is passed in the client context collection.
        /// </summary>
        public string PartitionKey { get; private set; }

        /// <summary>
        ///     How important this partition is (typically 1-10, but it's up to the user, highest weight wins).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used by <see cref="CalculateSeverity" />. Use it to weight the importance of different partitions.
        ///     </para>
        /// </remarks>
        public int Weight
        {
            get => _weight;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "Must be 1 or larger. 1 = no extra importance is given to this partition.");
                _weight = value;
            }
        }

        /// <summary>
        ///     Calculate the priority order for this partition (more is better)
        /// </summary>
        /// <param name="numberOfItemsAffectedByIncident">Number of unique items for the defined partition key</param>
        /// <param name="knownAmountOfItems">Number of unique items that we've received reports for in the application.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         Calculation can be done in two ways.
        ///     </para>
        ///     <para>
        ///         First is used if <see cref="NumberOfItems" /> is specified. Then we take the partition impact  and then weight
        ///         that. <c>(numberOfItemsAffectedByIncident/NumberOfItems)*Weight</c>
        ///     </para>
        ///     <para>
        ///         If <c>NumberOfItems</c> is not specified, we can't be sure if we've received incidents for all items in a
        ///         partition. Therefore any usage of percentage is not accurate. However,
        ///         we still use percentage to get a priority similar to those partitions that have got <c>NumberOfItems</c>
        ///         defined. But the severity calculation is only useful for all incidents
        ///         in the same partition (or when no partition have <see cref="NumberOfItems" /> defined).
        ///         we simply use the <c>numberOfAffectedItems * Weight</c>
        ///     </para>
        /// </remarks>
        public int CalculateSeverity(int numberOfItemsAffectedByIncident, int knownAmountOfItems)
        {
            if (numberOfItemsAffectedByIncident <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfItemsAffectedByIncident));
            if (NumberOfItems == 0 && knownAmountOfItems == 0)
                throw new ArgumentOutOfRangeException(nameof(knownAmountOfItems),
                    "Must be specified when the NumberOfItems property is not specified.");

            var maxValue = NumberOfItems == 0 ? knownAmountOfItems : NumberOfItems;
            var percentage = numberOfItemsAffectedByIncident * 100 / maxValue;
            return Weight == 0 ? percentage : percentage * Weight;
        }

        /// <summary>
        ///     Calculate amount of affected items
        /// </summary>
        /// <param name="numberOfItemsAffectedItems">Number of unique items for the defined partition key</param>
        public int CalculatePercentage(int numberOfItemsAffectedItems)
        {
            return numberOfItemsAffectedItems * 100 / NumberOfItems;
        }
    }
}