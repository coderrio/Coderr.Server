using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;
using Coderr.Server.App.Partitions;
using Coderr.Server.Domain.Modules.Partitions;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Partitions
{
    [ContainerService]
    internal class PartitionInsight : IKeyPerformanceIndicatorGenerator
    {
        private IPartitionRepository _partitionRepository;
        private IAdoNetUnitOfWork _unitOfWork;

        public PartitionInsight(IAdoNetUnitOfWork unitOfWork, IPartitionRepository partitionRepository)
        {
            _unitOfWork = unitOfWork;
            _partitionRepository = partitionRepository;
        }


        public async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var definitions = new Dictionary<int, IList<PartitionDefinition>>();
            foreach (var applicationId in context.ApplicationIds)
            {
                var partitions = await _partitionRepository.GetDefinitions(applicationId);
                definitions[applicationId] = partitions;
            }

            await FillKnownTotals(context, definitions);

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select ApplicationId, PartitionId, Count(Value) Count, YearMonth
                                    FROM ApplicationPartitionInsights
                                    JOIN PartitionDefinitions ON (PartitionDefinitions.Id = PartitionId)
                                    WHERE YearMonth > @from AND YearMonth <= @to
                                    #AppIdConstraint#
                                    GROUP BY ApplicationId, PartitionId, YearMonth";
                cmd.AddParameter("from", context.StartDate);
                cmd.AddParameter("to", context.EndDate);
                ApplyApplicationIdConstraint(cmd, context.ApplicationIds);

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

                        var perPartition = appRows.GroupBy(x => x.AccountId);
                        foreach (var partitionRow in perPartition)
                        {
                            var partition = partitions.FirstOrDefault(x => x.Id == partitionRow.Key);
                            if (partition == null)
                            {
                                continue;
                            }

                            if (partition.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                            {
                                partition.Name = "Affected " + partition.Name.Remove(partition.Name.Length - 2, 2).Pluralize();
                            }


                            var kpi = new KeyPerformanceIndicator("Partition" + partition.Id, partition.Name,
                                IndicatorValueComparison.LowerIsBetter)
                            {
                                PeriodValue = partitionRow
                                    .Where(x => x.When >= context.PeriodStartDate && x.When <= context.PeriodEndDate)
                                    .Sum(x => x.Count),
                                Value = partitionRow
                                    .Where(x => x.When >= context.ValueStartDate && x.When <= context.ValueEndDate)
                                    .Sum(x => x.Count),

                            };

                            kpi.Description = $"Amount of affected {partition.Name.ToLower()}";

                            var values = new TrendLineValue[context.TrendDates.Length];
                            for (var i = 0; i < context.TrendDates.Length; i++)
                            {
                                var value = partitionRow
                                    .Where(x => x.When == context.TrendDates[i])
                                    .DefaultIfEmpty()
                                    .Sum(x => x?.Count ?? 0);
                                values[i] = new TrendLineValue(value);
                            }

                            var line = new TrendLine(partition.Name, values);
                            kpi.TrendLines = new[] { line };
                            context.AddIndicator(appRows.Key, kpi);
                        }
                    }
                }
            }
        }

        private async Task FillKnownTotals(KeyPerformanceIndicatorContext context, Dictionary<int, IList<PartitionDefinition>> definitions)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT ApplicationId, PartitionId, Count(*) 
                                    FROM ApplicationPartitionValues 
                                    JOIN PartitionDefinitions ON (PartitionDefinitions.Id = PartitionId)
                                    #AppIdConstraint#
                                    GROUP BY ApplicationId, PartitionId";
                ApplyApplicationIdConstraint(cmd, context.ApplicationIds, " WHERE ");
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

        private static void ApplyApplicationIdConstraint(IDbCommand cmd, int[] applicationIds, string queryPrefix = " AND ")
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
    }
}