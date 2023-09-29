using System;
using System.Collections.Generic;

namespace Coderr.Server.Domain.Modules.Partitions
{
    /// <summary>
    ///     A identified partition for an incident
    /// </summary>
    public class IncidentPartitionValue : IEquatable<IncidentPartitionValue>
    {

        public IncidentPartitionValue(int partitionId, int incidentId, int value)
        {
            if (partitionId <= 0) throw new ArgumentOutOfRangeException(nameof(partitionId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            PartitionId = partitionId;
            IncidentId = incidentId;
            ValueId = value;
        }

        protected IncidentPartitionValue()
        {
        }

        public int Id { get; set; }
        /// <summary>
        ///     Incident that this partition information is for.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Which partition this information is for.
        /// </summary>
        public int PartitionId { get; private set; }

        public int ValueId { get; private set; }

        public bool Equals(IncidentPartitionValue other)
        {
            return other != null &&
                   IncidentId == other.IncidentId &&
                   PartitionId == other.PartitionId &&
                   ValueId == other.ValueId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IncidentPartitionValue);
        }

        public override int GetHashCode()
        {
            var hashCode = 973172448;
            hashCode = hashCode * -1521134295 + IncidentId.GetHashCode();
            hashCode = hashCode * -1521134295 + PartitionId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(ValueId);
            return hashCode;
        }
    }
}