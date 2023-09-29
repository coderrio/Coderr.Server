using System;

namespace Coderr.Server.Domain.Modules.Partitions
{
    /// <summary>
    ///     Tracks all partitions that have been uploaded for an application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class and <see cref="IncidentPartitionValue" /> differs since this one keeps track of all distinct
    ///         values, no matter which incident they was received for.
    ///         The purpose is to tell which incident affected most known values. i.e. if incident #2 reported for "adam" and
    ///         "anna" and incident #3 reported for "anna" we got two distinct values and
    ///         incident #2 affected 100% (both users) while incident #3 only affected (50%).
    ///     </para>
    ///     <para>
    ///         Since a partition is defined for a specific application, there is no application id in this table.
    ///     </para>
    /// </remarks>
    public class ApplicationPartitionValue
    {
        public ApplicationPartitionValue(int partitionId, string value)
        {
            if (partitionId <= 0) throw new ArgumentOutOfRangeException(nameof(partitionId));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            PartitionId = partitionId;
        }

        protected ApplicationPartitionValue()
        {
        }

        public int PartitionId { get; private set; }

        public string Value { get; private set; }
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ApplicationPartitionValue other))
                return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PartitionId * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        protected bool Equals(ApplicationPartitionValue other)
        {
            return PartitionId == other.PartitionId && string.Equals(Value, other.Value);
        }
    }
}