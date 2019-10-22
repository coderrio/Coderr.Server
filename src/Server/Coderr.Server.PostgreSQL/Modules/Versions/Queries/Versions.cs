using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Api.Modules.Versions.Queries;
using Coderr.Server.Infrastructure;

namespace Coderr.Server.PostgreSQL.Modules.Versions.Queries
{
    internal class Versions
    {
        private readonly Dictionary<string, VersionRange> _versions = new Dictionary<string, VersionRange>();

        public bool IsEmpty => _versions.Count == 0;

        public void AddCounts(string version, DateTime yearMonth, int incidentCount, int reportCount)
        {
            if (!_versions.TryGetValue(version, out var entity))
            {
                entity = new VersionRange();
                _versions[version] = entity;
            }

            entity.AddCounts(yearMonth, incidentCount, reportCount);
        }

        public GetVersionHistoryResultSet[] BuildIncidentSeries()
        {
            var items = new List<GetVersionHistoryResultSet>();
            foreach (var key in _versions.Keys)
            {
                items.Add(new GetVersionHistoryResultSet
                {
                    Name = key,
                    Values = _versions[key].IncidentSeries.ToArray()
                });
            }

            return items.OrderBy(x => x.Name, new ApplicationVersionComparer()).ToArray();
        }

        public GetVersionHistoryResultSet[] BuildReportSeries()
        {
            var items = new List<GetVersionHistoryResultSet>();
            foreach (var key in _versions.Keys)
            {
                items.Add(new GetVersionHistoryResultSet
                {
                    Name = key,
                    Values = _versions[key].ReportSeries.ToArray()
                });
            }

            return items.OrderBy(x => x.Name, new ApplicationVersionComparer()).ToArray();
        }

        public string[] GetDates()
        {
            var firstKey = _versions.Keys.First();
            return _versions[firstKey].Dates.Select(x => x.ToShortDateString()).ToArray();
        }

        public void PadMonths(DateTime from, DateTime to)
        {
            foreach (var version in _versions)
            {
                var current = from;
                while (current <= to)
                {
                    version.Value.EnsureMonth(current);
                    current = current.AddMonths(1);
                }
            }
        }
    }
}