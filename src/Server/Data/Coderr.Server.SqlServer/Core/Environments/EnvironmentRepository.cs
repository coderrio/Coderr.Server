using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Core.Environments;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Environments
{
    [ContainerService]
    internal class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public EnvironmentRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ApplicationEnvironment>> ListForApplication(int applicationId)
        {
            var sql = @"SELECT ae.*, Name
                        FROM Environments
                        JOIN ApplicationEnvironments ae ON (EnvironmentId=Environments.Id)
                        WHERE ApplicationId = @applicationId";

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("applicationId", applicationId);
                var items = await cmd.ToListAsync<ApplicationEnvironment>();
                return items.ToList();
            }
        }

        public async Task<IReadOnlyList<Environment>> ListAll()
        {
            string sql;
            sql = @"select Id, Name from Environments ORDER BY Name";

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = sql;
                var items = await cmd.ToListAsync<Environment>();
                return items.ToList();
            }
        }

        public async Task Create(Environment environment)
        {
            await _unitOfWork.InsertAsync(environment);
        }

        public async Task Delete(Environment environment)
        {
            await _unitOfWork.DeleteAsync(environment);
        }

        public async Task Create(ApplicationEnvironment environment)
        {
            await _unitOfWork.InsertAsync(environment);
        }

        public async Task Update(ApplicationEnvironment environment)
        {
            await _unitOfWork.UpdateAsync(environment);
        }

        public async Task<Environment> FindByName(string name)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"select *
                                    FROM Environments
                                    WHERE Name = @name";
                cmd.AddParameter("name", name);
                return await cmd.FirstOrDefaultAsync<Environment>();
            }
        }

        public async Task Reset(int environmentId, int? applicationId)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                // Start by deleting incidents that are in just our environment.
                cmd.CommandText = @"WITH JustOurIncidents (IncidentId) AS
                            (
	                            select ie.IncidentId
	                            from IncidentEnvironments ie 
	                            join Incidents i ON (i.Id = ie.IncidentId)
	                            join Environments e ON (ie.EnvironmentId = e.Id)
	                            where i.ApplicationId = @applicationId AND i.State = 0
	                            group by ie.IncidentId
	                            having count(e.Id) = 1
                            )
                            DELETE Incidents
                            FROM IncidentEnvironments
                            JOIN JustOurIncidents ON (JustOurIncidents.IncidentId = IncidentEnvironments.IncidentId)
                            WHERE IncidentEnvironments.EnvironmentId = @environmentId";

                cmd.AddParameter("environmentId", environmentId);
                cmd.AddParameter("applicationId", applicationId);
                await cmd.ExecuteNonQueryAsync();
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                // Next delete all environment mappings that are for the given environment.
                cmd.CommandText = @"WITH JustOurIncidents (IncidentId) AS
                            (
	                            select ie.IncidentId
	                            from IncidentEnvironments ie 
	                            join Incidents i ON (i.Id = ie.IncidentId)
	                            join Environments e ON (ie.EnvironmentId = e.Id)
	                            where i.ApplicationId = @applicationId AND i.State = 0
	                            group by ie.IncidentId
                            )
                            DELETE IncidentEnvironments
                            FROM IncidentEnvironments
                            JOIN JustOurIncidents ON (JustOurIncidents.IncidentId = IncidentEnvironments.IncidentId)
                            WHERE IncidentEnvironments.EnvironmentId = @environmentId";

                cmd.AddParameter("environmentId", environmentId);
                cmd.AddParameter("applicationId", applicationId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<ApplicationEnvironment> Find(int environmentId, int applicationId)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"select ae.*, e.Name 
                                    from ApplicationEnvironments ae 
                                    JOIN Environments e ON (e.Id = ae.EnvironmentId)
                                    WHERE EnvironmentId = @envId AND ApplicationId = @appId";
                cmd.AddParameter("appId", applicationId);
                cmd.AddParameter("envId", environmentId);
                return await cmd.FirstOrDefaultAsync<ApplicationEnvironment>();
            }
        }

        public async Task<Environment> Find(int environmentId)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"select *
                                    FROM Environments
                                    WHERE Id = @envId";
                cmd.AddParameter("envId", environmentId);
                return await cmd.FirstOrDefaultAsync<Environment>();
            }
        }
    }
}