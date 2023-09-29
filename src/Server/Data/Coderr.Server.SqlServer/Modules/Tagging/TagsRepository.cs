using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.Tags;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Modules.Tagging
{
    [ContainerService]
    public class TagsRepository : ITagsRepository
    {
        private readonly IAdoNetUnitOfWork _adoNetUnitOfWork;

        public TagsRepository(IAdoNetUnitOfWork adoNetUnitOfWork)
        {
            _adoNetUnitOfWork = adoNetUnitOfWork;
        }

        public async Task AddAsync(int incidentId, Tag[] tags)
        {
            foreach (var tag in tags)
            {
                using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO IncidentTags (IncidentId, TagName, OrderNumber) VALUES(@incidentId, @name, @orderNumber)";
                    cmd.AddParameter("incidentId", incidentId);
                    cmd.AddParameter("name", tag.Name);
                    cmd.AddParameter("orderNumber", tag.OrderNumber);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateTags(int incidentId, string[] tagsToAdd, string[] tagsToRemove)
        {
            foreach (var tag in tagsToAdd)
            {
                using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO IncidentTags (IncidentId, TagName, OrderNumber) VALUES(@incidentId, @name, 1)";
                    cmd.AddParameter("incidentId", incidentId);
                    cmd.AddParameter("name", tag);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            foreach (var tag in tagsToRemove)
            {
                using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "DELETE FROM IncidentTags WHERE IncidentId = @incidentId AND TagName = @name";
                    cmd.AddParameter("incidentId", incidentId);
                    cmd.AddParameter("name", tag);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AddTag(int incidentId, string tag)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "INSERT INTO IncidentTags (IncidentId, TagName, OrderNumber) VALUES(@incidentId, @name, 1)";
                cmd.AddParameter("incidentId", incidentId);
                cmd.AddParameter("name", tag);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IReadOnlyList<Tag>> GetIncidentTagsAsync(int incidentId)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT * FROM IncidentTags WHERE IncidentId = @id ORDER BY OrderNumber";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var tags = new List<Tag>();
                    while (await reader.ReadAsync())
                    {
                        var tag = new Tag((string)reader["TagName"], (int)reader["OrderNumber"]);
                        tags.Add(tag);
                    }
                    return tags;
                }
            }
        }

        public async Task<IReadOnlyList<Tag>> GetTagsAsync(int? applicationId, int? incidentId)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select TagName, min(OrderNumber)
                                    FROM IncidentTags
                                    INNER JOIN Incidents ON (IncidentTags.IncidentId=Incidents.Id)";
                if (incidentId != null)
                {
                    cmd.CommandText += " WHERE Incidents.Id = @incidentId";
                    cmd.AddParameter("@incidentId", incidentId.Value);
                }
                else if (applicationId != null)
                {
                    cmd.CommandText += " WHERE Incidents.ApplicationId = @applicationId";
                    cmd.AddParameter("appId", applicationId.Value);
                }

                cmd.CommandText += " GROUP BY TagName";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var tags = new List<Tag>();
                    while (await reader.ReadAsync())
                    {
                        var tag = new Tag((string)reader[0], (int)reader[1]);
                        tags.Add(tag);
                    }
                    return tags;
                }
            }
        }

        public async Task<IReadOnlyList<int>> GetNewIncidentsForTag(int? applicationId, string tag)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select it.IncidentId
                                    from incidents i
                                    join IncidentTags it ON (it.IncidentId = i.Id)
                                    WHERE it.TagName = @tagName and i.State = 0";
                if (applicationId != null)
                {
                    cmd.CommandText += " AND i.ApplicationId = @appId";
                    cmd.AddParameter("appId", applicationId.Value);
                }
                cmd.AddParameter("tagName", tag);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var incidentIds = new List<int>();
                    while (await reader.ReadAsync())
                    {
                        incidentIds.Add(reader.GetInt32(0));
                    }
                    return incidentIds;
                }
            }
        }


        public async Task<IReadOnlyList<Tag>> GetApplicationTagsAsync(int applicationId)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select TagName, min(OrderNumber)
                                    FROM IncidentTags
                                    INNER JOIN Incidents ON (IncidentTags.IncidentId=Incidents.Id)
                                    WHERE Incidents.ApplicationId = @id
                                    GROUP BY TagName";
                cmd.AddParameter("id", applicationId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var tags = new List<Tag>();
                    while (await reader.ReadAsync())
                    {
                        var tag = new Tag((string)reader[0], (int)reader[1]);
                        tags.Add(tag);
                    }
                    return tags;
                }
            }
        }
    }
}