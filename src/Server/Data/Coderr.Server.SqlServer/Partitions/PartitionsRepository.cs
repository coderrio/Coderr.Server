using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.Partitions;
using Coderr.Server.ReportAnalyzer.Partitions;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.Partitions
{
    [ContainerService]
    public class PartitionsRepository : IPartitionRepository
    {
        private readonly IAdoNetUnitOfWork _uow;
        private ILog _logger = LogManager.GetLogger(typeof(PartitionsRepository));

        public PartitionsRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task CreateAsync(InboundDTO dto)
        {
            await _uow.InsertAsync(dto);
        }

        public async Task CreateAsync(IncidentPartitionValue incidentPartitionValues)
        {
            await _uow.InsertAsync(incidentPartitionValues);
        }

        public async Task CreateAsync(PartitionDefinition definition)
        {
            await _uow.InsertAsync(definition);
        }

        public async Task CreateAsync(ApplicationPartitionValue applicationValues)
        {
            await _uow.InsertAsync(applicationValues);
        }

        public async Task<IList<ApplicationPartitionValue>> FindForApplication(int applicationId)
        {
            return await _uow.ToListAsync<ApplicationPartitionValue>("ApplicationId = @ApplicationId",
                new {ApplicationId = applicationId});
        }

        public async Task<IList<PartitionDefinition>> GetDefinitions(int applicationId)
        {
            return await _uow.ToListAsync<PartitionDefinition>("ApplicationId = @1", applicationId);
        }

        public async Task<IList<PartitionDefinition>> GetDefinitionsToTag()
        {
            return await _uow.ToListAsync<PartitionDefinition>("ImportantThreshold > @amount OR CriticalThreshold > @amount", new{amount = 0});
        }

        public async Task<IReadOnlyList<IncidentToTag>> GetIncidentsToTag(PartitionDefinition definition)
        {
            var threshold = Math.Min(definition.CriticalThreshold ?? int.MaxValue,
                definition.ImportantThreshold ?? int.MaxValue);
            if (threshold == int.MaxValue)
                return new IncidentToTag[0];

            var items = new List<IncidentToTag>();
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    @"select IncidentId, ApplicationId, COUNT(ipv.ValueId) ValueCount
                        FROM IncidentPartitionValues ipv
                        join incidents on (Incidents.Id = IncidentId)
                        WHERE ipv.PartitionId = @partitionId
                        GROUP BY ApplicationId, ipv.IncidentId, ipv.PartitionId
                        HAVING count(ipv.ValueId) > @threshold";

                cmd.AddParameter("partitionId", definition.Id);
                cmd.AddParameter("threshold", threshold);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var entity = new IncidentToTag
                        {
                            PartitionId = definition.Id,
                            IncidentId = reader.GetInt32(0),
                            ApplicationId = reader.GetInt32(1),
                            Count = reader.GetInt32(2),
                        };

                        items.Add(entity);
                    }
                }
            }

            if (items.Count == 0)
            {
                return new IncidentToTag[0];
            }
            
            var criticalIncidentsUpdate = new List<int>();
            var importantIncidentsUpdate = new List<int>();
            using (var cmd = _uow.CreateDbCommand())
            {
                var ids = string.Join(", ", items.Select(x => x.IncidentId));
                cmd.CommandText = $"SELECT IncidentId, IsCritical,IsImportant " +
                                  $"FROM PartitionTaggedIncidents " +
                                  $"WHERE IncidentId IN ({ids}) AND PartitionId = @partitionId";
                cmd.AddParameter("partitionId", definition.Id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var incidentId = reader.GetInt32(0);
                        var isCritical = reader.GetBoolean(1);
                        var isImportant = reader.GetBoolean(2);
                        var incident = items.FirstOrDefault(x => x.IncidentId == incidentId);
                        if (incident == null)
                            continue;


                        var isCurrentCritical = definition.CriticalThreshold != null && incident.Count > definition.CriticalThreshold;
                        var isCurrentImportant = definition.ImportantThreshold != null && incident.Count > definition.ImportantThreshold;

                        var updated = false;
                        if (isImportant != isCurrentImportant)
                        {
                            importantIncidentsUpdate.Add(incidentId);
                            updated = true;
                        }
                        if (isCritical != isCurrentCritical)
                        {
                            criticalIncidentsUpdate.Add(incidentId);
                            updated = true;
                        }

                        if (!updated)
                        {
                            items.Remove(incident);
                        }
                    }
                }
            }

            if (criticalIncidentsUpdate.Count > 0)
            {
                using (var cmd = _uow.CreateDbCommand())
                {
                    var ids = string.Join(", ", criticalIncidentsUpdate);
                    cmd.CommandText = $"UDPATE PartitionTaggedIncidents SET IsCritical=1 WHERE IncidentId IN ({ids})";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            if (importantIncidentsUpdate.Count > 0)
            {
                using (var cmd = _uow.CreateDbCommand())
                {
                    var ids = string.Join(", ", importantIncidentsUpdate);
                    cmd.CommandText = $"UDPATE PartitionTaggedIncidents SET IsImportant=1 WHERE IncidentId IN ({ids})";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return items;
        }

        public async Task<IList<IncidentPartitionSummary>> GetIncidentSummary(int applicationId, int incidentId)
        {
            var defs = await GetDefinitions(applicationId);

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    @"SELECT PartitionId, IncidentId, Count(IncidentPartitionValues.ValueId) IncidentCount, (SELECT Count(*) FROM ApplicationPartitionValues WHERE PartitionId=PartitionId) AppCount
                                    FROM IncidentPartitionValues
                                    WHERE IncidentId = @incidentId
                                    GROUP BY PartitionId, IncidentId";
                cmd.AddParameter("incidentId", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<IncidentPartitionSummary>();
                    while (await reader.ReadAsync())
                    {
                        var paritionId = (int) reader["PartitionId"];
                        var def = defs.FirstOrDefault(x => x.Id == paritionId);
                        if (def == null)
                        {
                            _logger.Warn("Failed to find definition " + paritionId);
                            continue;
                        }

                        var totalCount = def.NumberOfItems == 0 ? (int) reader["AppCount"] : def.NumberOfItems;

                        var entity = new IncidentPartitionSummary
                        {
                            Id = def.Id,
                            PartitionTitle = def.Name,
                            IncidentId = (int) reader["IncidentId"],
                            ValueCount = (int) reader["IncidentCount"],
                            TotalCount = totalCount
                        };

                        items.Add(entity);
                    }

                    return items;
                }
            }
        }

        public async Task<IEnumerable<IncidentPartitionSummary>> GetPrioritized(int[] incidentIds)
        {
            var defs = await GetDefinitions();

            string cteConstraint = "";
            if (incidentIds?.Length > 0)
            {
                cteConstraint = $" WHERE IncidentId IN ({string.Join(",", incidentIds)})";
            }

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = $@"with IncidentPartitions
                                    AS
                                    ( 
                                        SELECT TOP (10) PartitionId, IncidentId, Count(IncidentPartitionValues.ValueId) IncidentCount, 
                                                (SELECT Count(*) FROM ApplicationPartitionValues WHERE PartitionId=PartitionId) AppCount
                                        FROM IncidentPartitionValues {cteConstraint}
                                        GROUP BY IncidentId, PartitionId
                                        ORDER BY IncidentCount DESC
                                    )
                                    select pd.Id PartitionId, 
                                            case when pd.NumberOfItems is null 
                                                then (ip.IncidentCount*100/iif(ip.AppCount=0,1,ip.AppCount)*pd.Weight) 
                                                else (ip.IncidentCount*100/pd.NumberOfItems*pd.Weight) 
                                            end as Severity, 
                                            ip.IncidentCount, 
                                            ip.IncidentId, 
                                            pd.Name, 
                                            ip.AppCount
                                    FROM IncidentPartitions ip
                                    JOIN incidents i ON (i.id=ip.incidentId)
                                    JOIN PartitionDefinitions pd ON (pd.Id=PartitionId)
                                    WHERE i.state = 0
                                    ORDER BY Severity DESC";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<IncidentPartitionSummary>();
                    while (await reader.ReadAsync())
                    {
                        var partitionId = (int) reader["PartitionId"];
                        var def = defs.First(x => x.Id == partitionId);
                        var totalCount = def.NumberOfItems == 0 ? (int) reader["AppCount"] : def.NumberOfItems;

                        var entity = new IncidentPartitionSummary
                        {
                            Id = def.Id,
                            PartitionTitle = def.Name,
                            IncidentId = (int) reader["IncidentId"],
                            ValueCount = (int) reader["IncidentCount"],
                            TotalCount = totalCount,
                            Severity = (int) reader["Severity"]
                        };

                        items.Add(entity);
                    }

                    return items;
                }
            }
        }

        public async Task<IEnumerable<IncidentPartitionSummary>> GetPrioritized(int applicationId)
        {
            var defs = await GetDefinitions();
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = @"with IncidentPartitions
                                    AS
                                    ( 
                                        SELECT TOP (10) PartitionId, IncidentId, Count(IncidentPartitionValues.ValueId) IncidentCount, 
                                                (SELECT Count(*) FROM ApplicationPartitionValues WHERE PartitionId=PartitionId) AppCount
                                        FROM IncidentPartitionValues
                                        JOIN Incidents ON (Incidents.Id = IncidentPartitionValues.IncidentId)
                                        WHERE Incidents.ApplicationId = @appId
                                        AND Incidents.state = 0
                                        GROUP BY IncidentId, PartitionId
                                        ORDER BY IncidentCount DESC
                                    )
                                    select pd.Id PartitionId, case when pd.NumberOfItems is null then (ip.IncidentCount*100/ip.AppCount*pd.Weight) else (ip.IncidentCount*100/pd.NumberOfItems*pd.Weight) end as Severity, ip.IncidentCount, ip.IncidentId, pd.Name, ip.AppCount
                                    FROM IncidentPartitions ip
                                    JOIN PartitionDefinitions pd ON (pd.Id=PartitionId)
                                    ORDER BY Severity DESC";
                cmd.AddParameter("appId", applicationId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<IncidentPartitionSummary>();
                    while (await reader.ReadAsync())
                    {
                        var def = defs.First(x => x.Id == (int) reader["PartitionId"]);
                        var totalCount = def.NumberOfItems == 0 ? (int) reader["AppCount"] : def.NumberOfItems;

                        var entity = new IncidentPartitionSummary
                        {
                            Id = def.Id,
                            PartitionTitle = def.Name,
                            IncidentId = (int) reader["IncidentId"],
                            ValueCount = (int) reader["IncidentCount"],
                            TotalCount = totalCount,
                            Severity = (int) reader["Severity"]
                        };

                        items.Add(entity);
                    }

                    return items;
                }
            }
        }

        public async Task UpdateAsync(IncidentPartitionValue incidentPartitionValues)
        {
            await _uow.UpdateAsync(incidentPartitionValues);
        }

        public async Task UpdateAsync(PartitionDefinition definition)
        {
            await _uow.UpdateAsync(definition);
        }

        public async Task UpdateAsync(ApplicationPartitionValue applicationValues)
        {
            await _uow.UpdateAsync(applicationValues);
        }

        public async Task<PartitionDefinition> GetByIdAsync(int id)
        {
            return await _uow.FirstAsync<PartitionDefinition>(new {Id = id});
        }

        public async Task DeleteDefinitionByIdAsync(int partitionId)
        {
            await _uow.DeleteAsync<IncidentPartitionValue>(new { PartitionId = partitionId });
            await _uow.DeleteAsync<ApplicationPartitionValue>(new { PartitionId = partitionId });
            await _uow.DeleteAsync<PartitionDefinition>(new {Id = partitionId});
        }

        private async Task<IList<PartitionDefinition>> GetDefinitions()
        {
            return await _uow.ToListAsync<PartitionDefinition>("Id > 0");
        }
    }
}