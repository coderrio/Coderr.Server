using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Incidents;
using OneTrueError.App.Core.Incidents;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Incidents
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

        public IEnumerable<IncidentSummaryDTO> FindLatestForOrganization(int count)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP " + count + " * FROM Incidents WHERE IsSolved=0 ORDER BY UpdatedAtUtc DESC";


                return cmd.ToList<IncidentSummaryDTO>();
            }
        }

        public async Task UpdateAsync(Incident incident)
        {

            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    @"UPDATE Incidents SET 
                        ApplicationId = @ApplicationId,
                        UpdatedAtUtc = @UpdatedAtUtc,
                        Description = @Description,
                        Solution = @Solution,
                        IsSolved = @IsSolved,
                        IsSolutionShared = @IsSolutionShared,
                        IgnoreReports = @IgnoreReports,
                        IgnoringReportsSinceUtc = @IgnoringReportsSinceUtc,
                        IgnoringRequestedBy = @IgnoringRequestedBy
                        WHERE Id = @id";
                cmd.AddParameter("Id", incident.Id);
                cmd.AddParameter("ApplicationId", incident.ApplicationId);
                cmd.AddParameter("UpdatedAtUtc", incident.UpdatedAtUtc);
                cmd.AddParameter("Description", incident.Description);
                cmd.AddParameter("IgnoreReports", incident.IgnoreReports);
                cmd.AddParameter("IgnoringReportsSinceUtc", incident.IgnoringReportsSinceUtc.ToDbNullable());
                cmd.AddParameter("IgnoringRequestedBy", incident.IgnoringRequestedBy);
                cmd.AddParameter("Solution",
                    incident.Solution == null ? null : EntitySerializer.Serialize(incident.Solution));
                cmd.AddParameter("IsSolved", incident.IsSolved);
                cmd.AddParameter("IsSolutionShared", incident.IsSolutionShared);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> GetTotalCountForAppInfoAsync(int applicationId)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT CAST(count(*) as int) FROM Incidents WHERE ApplicationId = @ApplicationId";
                cmd.AddParameter("ApplicationId", applicationId);
                var result = (int)await cmd.ExecuteScalarAsync();
                return result;
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

        public Task<Incident> GetAsync(int id)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP 1 * FROM Incidents WHERE Id = @id";

                cmd.AddParameter("id", id);
                return cmd.FirstAsync(new IncidentMapper());
            }
        }

        public IEnumerable<IncidentSummaryDTO> FindLatestForApplication(int applicationId, int count)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP " + count +
                    " * FROM Incidents WHERE ApplicationId = @applicationId AND IsSolved=0 ORDER BY UpdatedAtUtc DESC";

                cmd.AddParameter("applicationId", applicationId);
                return cmd.ToList<IncidentSummaryDTO>();
            }
        }

        public IEnumerable<IncidentSummaryDTO> FindWithMostReportsForOrganization(int count)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP " + count + " * FROM Incidents WHERE IsSolved=0 ORDER BY Count DESC";

                return cmd.ToList <IncidentSummaryDTO>();
            }
        }
    }
}