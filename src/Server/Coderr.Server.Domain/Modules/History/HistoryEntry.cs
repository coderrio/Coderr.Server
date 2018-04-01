using System;
using System.ComponentModel;
using Coderr.Server.Domain.Core.Incidents;

namespace Coderr.Server.Domain.Modules.History
{
    public class HistoryEntry
    {
        public HistoryEntry(int incidentId, int? accountId, IncidentState state)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (!Enum.IsDefined(typeof(IncidentState), state))
                throw new InvalidEnumArgumentException(nameof(state), (int) state, typeof(IncidentState));
            if (accountId != null && accountId <= 0)
                throw new ArgumentOutOfRangeException("AccountId should either be unspecified (system account) or larger than 0.");

            IncidentId = incidentId;
            AccountId = accountId;
            IncidentState = state;
            CreatedAtUtc = DateTime.UtcNow;
        }

        protected HistoryEntry()
        {
        }

        /// <summary>
        ///     User that made the transition to the new state
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <c>null</c> if the system made the transition
        ///     </para>
        /// </remarks>
        public int? AccountId { get; private set; }

        /// <summary>
        ///     Which version that the change was made in.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        ///     when the incident changed to this state
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        public int Id { get; set; }
        public int IncidentId { get; private set; }
        public IncidentState IncidentState { get; private set; }
    }
}