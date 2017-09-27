using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Triggers.Domain;
using codeRR.Server.SqlServer.Tools;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Triggers
{
    [Component]
    public class TriggerRepository : ITriggerRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public TriggerRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IList<CollectionMetadata>> GetCollectionsAsync(int applicationId)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM CollectionMetadata WHERE ApplicationId = @id";

                cmd.AddParameter("id", applicationId);
                return await cmd.ToListAsync(new CollectionMetadataMapper());
            }
        }


        public async Task UpdateAsync(CollectionMetadata collection)
        {
            var props = EntitySerializer.Serialize(collection.Properties);

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"UPDATE CollectionMetadata SET Properties = @Properties
                                    WHERE Id = @id";

                cmd.AddParameter("Id", collection.Id);
                cmd.AddParameter("Properties", props);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateAsync(CollectionMetadata entity)
        {
            var props = EntitySerializer.Serialize(entity.Properties);

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO CollectionMetadata (Name, ApplicationId, Properties) VALUES(@Name, @ApplicationId, @Properties)";
                cmd.AddParameter("Name", entity.Name);
                cmd.AddParameter("ApplicationId", entity.ApplicationId);
                cmd.AddParameter("Properties", props);
                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task CreateAsync(Trigger trigger)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "INSERT INTO Triggers (ApplicationId, Name, Description, Rules, Actions, LastTriggerAction, RunForNewIncidents, RunForExistingIncidents) " +
                    "VALUES(@ApplicationId, @Name, @Description, @Rules, @Actions, @LastTriggerAction, @RunForNewIncidents, @RunForExistingIncidents)";
                cmd.AddParameter("ApplicationId", trigger.ApplicationId);
                cmd.AddParameter("Name", trigger.Name);
                cmd.AddParameter("Description", trigger.Description);
                cmd.AddParameter("Rules", EntitySerializer.Serialize(trigger.Rules));
                cmd.AddParameter("Actions", EntitySerializer.Serialize(trigger.Actions));
                cmd.AddParameter("LastTriggerAction", trigger.LastTriggerAction);
                cmd.AddParameter("RunForNewIncidents", trigger.RunForNewIncidents);
                cmd.AddParameter("RunForExistingIncidents", trigger.RunForExistingIncidents);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public IEnumerable<Trigger> GetForApplication(int applicationId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM Triggers WHERE ApplicationId = @applicationId";
                cmd.AddParameter("applicationId", applicationId);
                return cmd.ToList(new TriggerMapper()).ToList();
            }
        }


        public async Task<Trigger> GetAsync(int id)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM Triggers WHERE Id = @id";
                cmd.AddParameter("id", id);
                return await cmd.FirstAsync(new TriggerMapper());
            }
        }

        public async Task UpdateAsync(Trigger trigger)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "UPDATE Triggers SET Name=@Name, Description=@Description, Rules=@Rules, Actions=@Actions, LastTriggerAction=@LastTriggerAction, RunForNewIncidents = @RunForNewIncidents, RunForExistingIncidents=@RunForExistingIncidents " +
                    " WHERE Id=@Id";
                cmd.AddParameter("Id", trigger.Id);
                cmd.AddParameter("Name", trigger.Name);
                cmd.AddParameter("Description", trigger.Description);
                cmd.AddParameter("Rules", EntitySerializer.Serialize(trigger.Rules));
                cmd.AddParameter("Actions", EntitySerializer.Serialize(trigger.Actions));
                cmd.AddParameter("LastTriggerAction", trigger.LastTriggerAction);
                cmd.AddParameter("RunForNewIncidents", trigger.RunForNewIncidents);
                cmd.AddParameter("RunForExistingIncidents", trigger.RunForExistingIncidents);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "DELETE FROM Triggers" +
                    " WHERE Id=@Id";
                cmd.AddParameter("Id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}