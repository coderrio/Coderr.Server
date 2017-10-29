using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents;
using codeRR.Server.App.Core.Incidents;
using codeRR.Server.SqlServer.Tools;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents
{
    [Component]
    public class IncidentRepository : IIncidentRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public IncidentRepository(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");

            _uow = uow;
        }

        public async Task UpdateAsync(Incident incident)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    @"UPDATE Incidents SET 
                        ApplicationId = @ApplicationId,
                        UpdatedAtUtc = @UpdatedAtUtc,
                        Description = @Description,
                        Solution = @Solution,
                        IsSolutionShared = @IsSolutionShared,
                        AssignedToId = @AssignedTo,
                        AssignedAtUtc = @AssignedAtUtc,
                        State = @state,
                        IgnoringReportsSinceUtc = @IgnoringReportsSinceUtc,
                        IgnoringRequestedBy = @IgnoringRequestedBy
                        WHERE Id = @id";
                cmd.AddParameter("Id", incident.Id);
                cmd.AddParameter("ApplicationId", incident.ApplicationId);
                cmd.AddParameter("UpdatedAtUtc", incident.UpdatedAtUtc);
                cmd.AddParameter("Description", incident.Description);
                cmd.AddParameter("State", (int)incident.State);
                cmd.AddParameter("AssignedTo", incident.AssignedToId);
                cmd.AddParameter("AssignedAtUtc", incident.AssignedAtUtc);
                cmd.AddParameter("IgnoringReportsSinceUtc", incident.IgnoringReportsSinceUtc.ToDbNullable());
                cmd.AddParameter("IgnoringRequestedBy", incident.IgnoringRequestedBy);
                cmd.AddParameter("Solution",
                    incident.Solution == null ? null : EntitySerializer.Serialize(incident.Solution));
                cmd.AddParameter("IsSolutionShared", incident.IsSolutionShared);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> GetTotalCountForAppInfoAsync(int applicationId)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT CAST(count(*) as int) FROM Incidents WHERE ApplicationId = @ApplicationId";
                cmd.AddParameter("ApplicationId", applicationId);
                var result = (int) await cmd.ExecuteScalarAsync();
                return result;
            }
        }

        public Task<Incident> GetAsync(int id)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP 1 * FROM Incidents WHERE Id = @id";

                cmd.AddParameter("id", id);
                return cmd.FirstAsync(new IncidentMapper());
            }
        }

        public Incident Find(int id)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP 1 * FROM Incidents WHERE Id = @id";

                cmd.AddParameter("id", id);
                return cmd.FirstOrDefault(new IncidentMapper());
            }
        }
        
        public Incident Get(int id)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP 3 * FROM Incidents WHERE Id = @id";

                cmd.AddParameter("id", id);
                return cmd.First(new IncidentMapper());
            }
        }
    }
}