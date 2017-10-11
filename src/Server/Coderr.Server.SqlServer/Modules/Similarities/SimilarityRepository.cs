using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Similarities.Domain;
using codeRR.Server.Infrastructure;
using codeRR.Server.SqlServer.Modules.Similarities.Entities;
using codeRR.Server.SqlServer.Tools;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace codeRR.Server.SqlServer.Modules.Similarities
{
    [Component]
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
                        var col = new SimilarityCollection(incidentId, reader.GetString(1));
                        col.GetType().GetProperty("Id").SetValue(col, reader.GetInt32(0));
                        foreach (var entity in properties)
                        {
                            var prop = new Similarity(entity.Name);
                            prop.LoadValues(
                                entity.Values.Select(x => new SimilarityValue(x.Value, x.Percentage, x.Count)).ToArray());
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