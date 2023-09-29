using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.IncidentInsights;
using Griffin.Container;

namespace Coderr.Server.App.Insights.Keyfigures.Application
{
    /// <summary>
    /// Generates application indicator for application versions
    /// </summary>
    [ContainerService]
    internal class VersionIndicator : IKeyPerformanceIndicatorGenerator
    {
        private const string IndicatorName = "ApplicationVersions";
        private readonly IIncidentProgressRepository _progressRepository;

        public VersionIndicator(IIncidentProgressRepository progressRepository)
        {
            _progressRepository = progressRepository;
        }

        public async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var incidents =
                await _progressRepository.FindCreated(context.ApplicationIds, context.StartDate, context.EndDate);

            foreach (var applicationId in context.ApplicationIds)
            {
                var appIncidents = incidents.Where(x => x.ApplicationId == applicationId);
                var versionIncidents = new List<VersionItem>();
                foreach (var incident in appIncidents)
                {
                    var versions = incident.Versions
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(version => new VersionItem
                        {
                            When = incident.CreatedAtUtc,
                            Version = version
                        });
                    versionIncidents.AddRange(versions);
                }

                var periodValue = versionIncidents
                    .Where(x => x.When >= context.PeriodStartDate && x.When <= context.PeriodEndDate)
                    .GroupBy(x => x.Version)
                    .Count();
                var values = versionIncidents
                    .Where(x => x.When >= context.ValueStartDate && x.When <= context.ValueEndDate)
                    .GroupBy(x => x.Version);
                var toplist = values.Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    Value = x.Count(),
                    ValueName = x.Key
                });
                var versionNames
                    = versionIncidents.Select(x => x.Version).Distinct();



                var trendLines = CreateTrendLines(context.TrendDates, versionIncidents);
                context.AddIndicator(applicationId, new KeyPerformanceIndicator(IndicatorName, "Application versions", IndicatorValueComparison.DoNotCompare)
                {
                    Value = values.Count(),
                    PeriodValue = periodValue,
                    TrendLines = trendLines.ToArray(),
                    ValueUnit = "versions",
                    Description = "Number of versions that received new incidents.",
                    Comment = "Versions: " + string.Join(", ", versionNames),
                    Toplist = toplist.ToArray()
                });
            }
        }

        private static List<TrendLine> CreateTrendLines(DateTime[] trendDates, List<VersionItem> versionIncidents)
        {
            var trendLines = new List<TrendLine>();
            for (var i = 0; i < trendDates.Length; i++)
            {
                var start = trendDates[i];
                var end = start.AddMonths(1).AddSeconds(-1);

                var values = versionIncidents
                    .Where(x => x.When >= start && x.When <= end)
                    .GroupBy(x => x.Version);
                foreach (var version in values)
                {
                    var line = trendLines.FirstOrDefault(x => x.DisplayName == version.Key);
                    if (line == null)
                    {
                        line = new TrendLine(version.Key, trendDates.Length, 0);
                        line.Fill(0);
                        trendLines.Add(line);
                    }

                    line.Assign(i, version.Count());
                }
            }

            return trendLines;
        }

        private class VersionItem
        {
            public DateTime When { get; set; }
            public string Version { get; set; }
        }
    }
}