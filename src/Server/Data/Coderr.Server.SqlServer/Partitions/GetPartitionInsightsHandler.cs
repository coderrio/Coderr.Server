using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Queries;
using Coderr.Server.App;
using Coderr.Server.App.Insights.Keyfigures.Application;
using Coderr.Server.App.Partitions;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Partitions
{
    public class GetPartitionInsightsHandler : IQueryHandler<GetPartitionInsights, GetPartitionInsightsResult>
    {
        private readonly IPartitionRepository _partitionRepository;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetPartitionInsightsHandler(IAdoNetUnitOfWork unitOfWork, IPartitionRepository partitionRepository)
        {
            _unitOfWork = unitOfWork;
            _partitionRepository = partitionRepository;
        }

        public async Task<GetPartitionInsightsResult> HandleAsync(IMessageContext context, GetPartitionInsights query)
        {
            query.StartDate = query.StartDate.ToFirstDayOfMonth();
            query.SummarizePeriodStartDate = query.SummarizePeriodStartDate.ToFirstDayOfMonth();

            var definitions = new Dictionary<int, IList<PartitionDefinition>>();
            foreach (var applicationId in query.ApplicationIds)
            {
                var partitions = await _partitionRepository.GetDefinitions(applicationId);
                definitions[applicationId] = partitions;
            }

            await FillKnownTotals(query.ApplicationIds, definitions);

            var resultApplications = new List<GetPartitionInsightsResultApplication>();
            var trendDates = CreateTrendDates(query.StartDate, query.EndDate);
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                if (query.IncidentId > 0)
                {
                    cmd.CommandText =
                        @"select 1, PartitionId, Count(ValueId) Count, DATEADD(MONTH, DATEDIFF(MONTH, 0, ReceivedAtUtc), 0) AS YearMonth
                          FROM IncidentPartitionValues
                          WHERE IncidentId = @incidentId
                            AND ReceivedAtUtc >= @from AND ReceivedAtUtc <= @to
                          GROUP BY PartitionId, DATEADD(MONTH, DATEDIFF(MONTH, 0, ReceivedAtUtc), 0)";
                    cmd.AddParameter("incidentId", query.IncidentId);
                }
                else
                {
                    cmd.CommandText = @"select ApplicationId, PartitionId, Count(Value) Count, YearMonth
                                    FROM ApplicationPartitionInsights
                                    JOIN PartitionDefinitions ON (PartitionDefinitions.Id = PartitionId)
                                    WHERE YearMonth >= @from AND YearMonth <= @to
                                    #AppIdConstraint#
                                    GROUP BY ApplicationId, PartitionId, YearMonth";
                    ApplyApplicationIdConstraint(cmd, query.ApplicationIds);
                }
                cmd.AddParameter("from", query.StartDate);
                cmd.AddParameter("to", query.EndDate);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<ItemWithCount>();
                    while (await reader.ReadAsync())
                    {
                        var item = new ItemWithCount(reader.GetInt32(0), reader.GetDateTime(3))
                        {
                            Count = reader.GetInt32(2),
                            AccountId = reader.GetInt32(1)
                        };
                        items.Add(item);
                    }

                    var perApplication = items.GroupBy(x => x.ApplicationId);

                    foreach (var appRows in perApplication)
                    {
                        var partitions = definitions[appRows.Key];

                        var indicators = new List<GetPartitionInsightsResultIndicator>();
                        var perPartition = appRows.GroupBy(x => x.AccountId);
                        foreach (var partitionRow in perPartition)
                        {
                            var partition = partitions.FirstOrDefault(x => x.Id == partitionRow.Key);
                            if (partition == null) continue;

                            if (partition.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                                partition.Name = "Affected " +
                                                 partition.Name.Remove(partition.Name.Length - 2, 2).Pluralize();

                            var monthStartDate = DateTime.Today.AddDays(-30);
                            var kpi = new GetPartitionInsightsResultIndicator("Partition" + partition.Id,
                                partition.Name)
                            {
                                PeriodValue = partitionRow
                                    .Where(x => x.When >= query.SummarizePeriodStartDate &&
                                                x.When <= query.SummarizePeriodEndDate)
                                    .Sum(x => x.Count),
                                Value = partitionRow
                                    .Where(x => x.When >= monthStartDate && x.When <= DateTime.UtcNow)
                                    .Sum(x => x.Count)
                            };

                            var values = new string[trendDates.Length];
                            for (var i = 0; i < trendDates.Length; i++)
                            {
                                var value = partitionRow
                                    .Where(x => x.When == trendDates[i])
                                    .DefaultIfEmpty()
                                    .Sum(x => x?.Count ?? 0);
                                values[i] = value.ToString();
                            }

                            kpi.Values = values;
                            kpi.Dates = trendDates;
                            indicators.Add(kpi);
                        }

                        resultApplications.Add(new GetPartitionInsightsResultApplication(appRows.Key)
                        {
                            Indicators = indicators.ToArray()
                        });
                    }
                }
            }

            return new GetPartitionInsightsResult { Applications = resultApplications.ToArray() };
        }

        private static void ApplyApplicationIdConstraint(IDbCommand cmd, int[] applicationIds,
            string queryPrefix = " AND ")
        {
            if (applicationIds.Length == 0)
            {
                cmd.CommandText = cmd.CommandText.Replace("#AppIdConstraint#", "");
            }
            else
            {
                var commaIds = string.Join(", ", applicationIds);
                cmd.CommandText = cmd.CommandText.Replace("#AppIdConstraint#",
                    $" {queryPrefix} ApplicationId IN ({commaIds}) ");
            }
        }

        private DateTime[] CreateTrendDates(DateTime startDate, DateTime endDate)
        {
            var date = startDate.ToFirstDayOfMonth();
            endDate = endDate.ToFirstDayOfMonth();
            var dates = new List<DateTime>();
            while (date <= endDate)
            {
                dates.Add(date);
                date = date.AddMonths(1);
            }

            return dates.ToArray();
        }

        private async Task FillKnownTotals(int[] applicationIds,
            Dictionary<int, IList<PartitionDefinition>> definitions)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT ApplicationId, PartitionId, Count(*) 
                                    FROM ApplicationPartitionValues 
                                    JOIN PartitionDefinitions ON (PartitionDefinitions.Id = PartitionId)
                                    #AppIdConstraint#
                                    GROUP BY ApplicationId, PartitionId";
                ApplyApplicationIdConstraint(cmd, applicationIds, " WHERE ");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var appId = reader.GetInt32(0);
                        var definitionId = reader.GetInt32(1);
                        var definition = definitions[appId].FirstOrDefault(x => x.Id == definitionId);
                        if (definition != null && definition.NumberOfItems == 0)
                            definition.NumberOfItems = reader.GetInt32(2);
                    }
                }
            }
        }
    }
}