using System;

namespace Coderr.Server.Api.Modules.Logs.Commands
{
    [Command]
    public class StoreLogEntries
    {
        public StoreLogEntries(int incidentId, int reportId, StoreLogEntriesEntry[] entries)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (reportId <= 0) throw new ArgumentOutOfRangeException(nameof(reportId));
            IncidentId = incidentId;
            ReportId = reportId;
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        protected StoreLogEntries()
        {

        }

        public StoreLogEntriesEntry[] Entries { get; private set; }
        public int IncidentId { get; private set; }
        public int ReportId { get; private set; }
    }
}