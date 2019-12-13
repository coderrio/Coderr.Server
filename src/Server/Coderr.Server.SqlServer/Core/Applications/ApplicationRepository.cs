using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.Applications;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    [ContainerService]
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public ApplicationRepository(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");
            _uow = uow;
        }

        public async Task CreateAsync(ApplicationTeamMember member)
        {
            await _uow.InsertAsync(member);
        }

        public async Task<UserApplication[]> GetForUserAsync(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = @"SELECT a.Id ApplicationId, a.Name ApplicationName, ApplicationMembers.Roles, a.NumberOfFtes NumberOfDevelopers
                                        FROM Applications a
                                        JOIN ApplicationMembers ON (ApplicationMembers.ApplicationId = a.Id) 
                                        WHERE ApplicationMembers.AccountId = @userId
                                        ORDER BY Name";
                cmd.AddParameter("userId", accountId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var apps = new List<UserApplication>();
                    while (await reader.ReadAsync())
                    {
                        var numberOfDevelopers = reader.GetValue(3);
                        var a = new UserApplication
                        {
                            IsAdmin = reader.GetString(2).Contains("Admin"),
                            ApplicationName = reader.GetString(1),
                            ApplicationId = reader.GetInt32(0),
                            NumberOfDevelopers = numberOfDevelopers is DBNull ? null : (decimal?)numberOfDevelopers
                        };
                        apps.Add(a);
                    }

                    return apps.ToArray();
                }
            }
        }

        public async Task RemoveTeamMemberAsync(int applicationId, string invitedEmailAddress)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM ApplicationMembers WHERE ApplicationId=@appId AND EmailAddress = @email";
                cmd.AddParameter("appId", applicationId);
                cmd.AddParameter("email", invitedEmailAddress);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(ApplicationTeamMember member)
        {
            await _uow.UpdateAsync(member);
        }

        public async Task<IList<ApplicationTeamMember>> GetTeamMembersAsync(int applicationId)
        {
            return await _uow.ToListAsync<ApplicationTeamMember>(@"SELECT Users.UserName, ApplicationMembers.* 
                                                                    FROM ApplicationMembers 
                                                                    LEFT JOIN Users ON (Users.AccountId = ApplicationMembers.AccountId) 
                                                                    WHERE ApplicationId = @1", applicationId);
        }


        public async Task<Application> GetByKeyAsync(string appKey)
        {
            if (appKey == null) throw new ArgumentNullException("appKey");

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM Applications WHERE AppKey = @id";

                cmd.AddParameter("id", appKey);
                var item = await cmd.FirstOrDefaultAsync(new ApplicationMapper());
                if (item == null)
                    throw new EntityNotFoundException(appKey, cmd);
                return item;
            }
        }

        /*Id uniqueidentifier not null primary key,
	Title nvarchar(50) not null,
	AppKey uniqueidentifier not null,
	OrganizationId uniqueidentifier not null,
	CreatedById uniqueidentifier not null,
	CreatedAtUtc datetime2 not null,
	ApplicationType varchar(40) not null,
	SharedSecret uniqueidentifier not null,*/

        public async Task<Application> GetByIdAsync(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("id");

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM Applications WHERE Id = @id";

                cmd.AddParameter("id", id);
                var item = await cmd.FirstOrDefaultAsync<Application>();
                if (item == null)
                    throw new EntityNotFoundException("Failed to find application with id " + id, cmd);

                return item;
            }
        }

        public async Task CreateAsync(Application application)
        {
            if (application == null) throw new ArgumentNullException("application");

            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO Applications (Name, AppKey, CreatedById, CreatedAtUtc, ApplicationType, SharedSecret, EstimatedNumberOfErrors, NumberOfFtes) 
                        VALUES(@Name, @AppKey, @CreatedById, @CreatedAtUtc, @ApplicationType, @SharedSecret, @EstimatedNumberOfErrors, @NumberOfFtes);SELECT SCOPE_IDENTITY();";
                cmd.AddParameter("Name", application.Name);
                cmd.AddParameter("AppKey", application.AppKey);
                cmd.AddParameter("CreatedById", application.CreatedById);
                cmd.AddParameter("CreatedAtUtc", application.CreatedAtUtc);
                cmd.AddParameter("ApplicationType", application.ApplicationType.ToString());
                cmd.AddParameter("SharedSecret", application.SharedSecret);
                cmd.AddParameter("EstimatedNumberOfErrors", application.EstimatedNumberOfErrors);
                cmd.AddParameter("NumberOfFtes", application.NumberOfFtes);
                var item = (decimal) await cmd.ExecuteScalarAsync();
                application.GetType().GetProperty("Id").SetValue(application, (int) item);
            }
        }


        public async Task DeleteAsync(int applicationId)
        {
            if (applicationId == 0) throw new ArgumentNullException("applicationId");

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    "DELETE FROM Applications WHERE Id = @id";

                //TODO: Delete reports??
                // or save them for future analysis?

                cmd.AddParameter("id", applicationId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<Application[]> GetAllAsync()
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Applications ORDER BY Name";

                //cmd.AddParameter("ids", string.Join(", ", appIds.Select(x => "'" + x + "'")));
                var result = await cmd.ToListAsync<Application>();
                return result.ToArray();
            }
        }

        public async Task UpdateAsync(Application entity)
        {
            await _uow.UpdateAsync(entity);
        }

        public async Task RemoveTeamMemberAsync(int applicationId, int userId)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM ApplicationMembers WHERE ApplicationId=@appId AND AccountId = @userId";
                cmd.AddParameter("appId", applicationId);
                cmd.AddParameter("userId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(Application application)
        {
            if (application == null) throw new ArgumentNullException("application");
            await DeleteAsync(application.Id);
        }
    }
}