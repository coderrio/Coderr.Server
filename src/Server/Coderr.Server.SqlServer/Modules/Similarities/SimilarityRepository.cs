using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.Similarities;
using Coderr.Server.Infrastructure;
using Coderr.Server.ReportAnalyzer.Similarities.Handlers.Processing;
using Coderr.Server.SqlServer.Modules.Similarities.Entities;
using Coderr.Server.SqlServer.Tools;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using log4net;

namespace Coderr.Server.SqlServer.Modules.Similarities
{
    [ContainerService]
    public class SimilarityRepository : ISimilarityRepository
    {
        private readonly IAdoNetUnitOfWork _uow;
        private ILog _logger = LogManager.GetLogger(typeof(SimilarityRepository));

        public SimilarityRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task CreateAsync(SimilaritiesReport similarity)
        {
            foreach (var collection in similarity.Collections)
            {
                var dto = collection.Properties.Select(x => new ContextCollectionPropertyDbEntity(x)).ToArray();
                var json = EntitySerializer.Serialize(dto);
                if (collection.Id == 0)
                {
                    using (var cmd = _uow.CreateDbCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO IncidentContextCollections (IncidentId, Name, Properties) 
                      VALUES(@incidentId, @name, @props)";
                        cmd.AddParameter("incidentId", collection.IncidentId);
                        cmd.AddParameter("name", collection.Name);
                        cmd.AddParameter("props", json);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    using (var cmd = _uow.CreateDbCommand())
                    {
                        cmd.CommandText =
                            @"UPDATE IncidentContextCollections SET Properties=@props WHERE Id = @id";
                        cmd.AddParameter("id", collection.Id);
                        cmd.AddParameter("props", json);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public SimilaritiesReport FindForIncident(int incidentId)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    @"select Id, Name, Properties from IncidentContextCollections 
                            where IncidentId = @incidentId";
                cmd.AddParameter("incidentId", incidentId);

                var collections = new List<SimilarityCollection>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var json = (string) reader["Properties"];
                        var properties = CoderrDtoSerializer.Deserialize<ContextCollectionPropertyDbEntity[]>(json);
                        foreach (var property in properties)
                        {
                            var zeroProps = property.Values.Where(x => x.Count == 0);
                            foreach (var prop in zeroProps)
                            {
                                _logger.Warn(
                                    $"Similarity with 0 count. IncidentId {incidentId}, Name {property.Name}, Value: {prop.Value}");
                                prop.Count = 1;
                            }
                        }

                        var col = new SimilarityCollection(incidentId, reader.GetString(1));
                        col.GetType().GetProperty("Id").SetValue(col, reader.GetInt32(0));
                        foreach (var entity in properties)
                        {
                            var values = entity.Values
                                .Select(x => new SimilarityValue(x.Value, x.Percentage, x.Count))
                                .ToArray();
                            var prop = new Similarity(entity.Name);
                            prop.LoadValues(values);
                            col.Properties.Add(prop);
                        }
                        collections.Add(col);
                    }
                }

                return collections.Count == 0 ? null : new SimilaritiesReport(incidentId, collections);
            }
        }

        public async Task UpdateAsync(SimilaritiesReport similarity)
        {
            await CreateAsync(similarity);
        }
    }
}