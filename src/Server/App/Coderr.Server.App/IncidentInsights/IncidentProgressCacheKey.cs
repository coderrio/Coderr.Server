using System;

namespace Coderr.Server.App.IncidentInsights
{
    public class IncidentProgressCacheKey : IEquatable<IncidentProgressCacheKey>
    {
        private readonly int[] _applicationIds;
        private readonly DateTime _from;
        private readonly DateTime _to;

        public IncidentProgressCacheKey(int applicationId, DateTime from, DateTime to)
        {
            _applicationIds = new[] {applicationId};
            _from = from;
            _to = to;
        }

        public IncidentProgressCacheKey(int[] applicationIds, DateTime from, DateTime to)
        {
            _applicationIds = applicationIds ?? throw new ArgumentNullException(nameof(applicationIds));
            _from = from;
            _to = to;
        }

        public bool Equals(IncidentProgressCacheKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_applicationIds, other._applicationIds)
                   && _from.Equals(other._from)
                   && _to.Equals(other._to);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IncidentProgressCacheKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _applicationIds.GetHashCode();
                hashCode = (hashCode * 397) ^ _from.GetHashCode();
                hashCode = (hashCode * 397) ^ _to.GetHashCode();
                return hashCode;
            }
        }
    }
}