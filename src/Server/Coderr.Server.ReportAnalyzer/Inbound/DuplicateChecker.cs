using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.ReportAnalyzer.Inbound.Models;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    public class DuplicateChecker
    {
        private Dictionary<string, DateTime> _lastReportIndex = new Dictionary<string, DateTime>();

        public DuplicateChecker()
        {
        }

        public bool IsDuplicate(string remoteAddress, NewReportDTO report)
        {
            lock (_lastReportIndex)
            {
                // duplicate
                if (_lastReportIndex.TryGetValue(report.ReportId, out var when))
                {
                    _lastReportIndex[report.ReportId] = DateTime.UtcNow;
                    return true;
                }

                if (_lastReportIndex.Count >= 100)
                {
                    var idsToRemove = _lastReportIndex.OrderBy(x => x.Value).Take(10);
                    foreach (var id in idsToRemove)
                    {
                        _lastReportIndex.Remove(id.Key);
                    }

                    _lastReportIndex[report.ReportId] = DateTime.UtcNow;
                }
            }

            return false;
        }
    }
}