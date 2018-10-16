using System;
using System.Collections.Generic;

namespace Coderr.Server.SqlServer.Modules.Versions.Queries
{
    internal class VersionRange
    {
        private readonly List<int> _incdentRange = new List<int>();
        private readonly Dictionary<DateTime, int> _incidents = new Dictionary<DateTime, int>();
        private readonly List<int> _reportRange = new List<int>();
        private readonly Dictionary<DateTime, int> _reports = new Dictionary<DateTime, int>();
        private List<DateTime> _dates = new List<DateTime>();
        public IEnumerable<int> IncidentSeries => _incdentRange;

        public IEnumerable<int> ReportSeries => _reportRange;

        public IEnumerable<DateTime> Dates => _dates;
        public string Version { get; set; }

        public void AddCounts(DateTime yearMonth, int incidentCount, int reportCount)
        {
            _reports[yearMonth] = reportCount;
            _incidents[yearMonth] = incidentCount;
        }

        public void EnsureMonth(DateTime yearMonth)
        {
            _dates.Add(yearMonth);
            if (!_incidents.TryGetValue(yearMonth, out var count))
                _incdentRange.Add(0);
            else
                _incdentRange.Add(count);

            if (!_reports.TryGetValue(yearMonth, out var count1))
                _reportRange.Add(0);
            else
                _reportRange.Add(count1);
        }
    }
}