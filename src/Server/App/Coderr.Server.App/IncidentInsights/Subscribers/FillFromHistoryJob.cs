using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.App.IncidentInsights.Subscribers
{
    
    /// <summary>
    /// Invoked at start to load incident history into stats.
    /// </summary>
    [ContainerService(RegisterAsSelf = true)]
    class FillFromHistoryJob : IBackgroundJob
    {
        private readonly IAdoNetUnitOfWork _adoNetUnitOfWork;
        private readonly IConfiguration<InsightConfiguration> _configuration;

        public FillFromHistoryJob(IAdoNetUnitOfWork adoNetUnitOfWork, IConfiguration<InsightConfiguration> configuration)
        {
            _adoNetUnitOfWork = adoNetUnitOfWork;
            _configuration = configuration;
        }

        public void Execute()
        {
            if (_configuration.Value.HaveFilledDatabase)
                return;

            using (var cmd = _adoNetUnitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO CommonIncidentProgressTracking (ApplicationId, IncidentId, CreatedAtUtc, VersionCount, Versions)
                        SELECT i.ApplicationId, ih.IncidentId, ih.CreatedAtUtc, 0, ''
                        FROM IncidentHistory ih
                        JOIN Incidents i ON (i.Id = ih.IncidentId)
                        LEFT JOIN CommonIncidentProgressTracking cipt ON (cipt.IncidentId = i.Id)
                        WHERE ih.State = 0 AND cipt.IncidentId IS NULL;

                        UPDATE CommonIncidentProgressTracking
                        SET Versions= CONCAT(';', av.Version, ';'), VersionCount=VersionCount+1
                        FROM IncidentVersions iv
                        JOIN ApplicationVersions av ON (av.Id = iv.VersionId)
                        WHERE CommonIncidentProgressTracking.IncidentId=iv.IncidentId

                        UPDATE CommonIncidentProgressTracking
                        SET AssignedAtUtc=ih.CreatedAtUtc, AssignedToId=ih.AccountId
                        FROM IncidentHistory ih
                        WHERE ih.IncidentId = CommonIncidentProgressTracking.IncidentId
                        AND ih.State = 1;

                        UPDATE CommonIncidentProgressTracking
                        SET ClosedAtUtc=ih.CreatedAtUtc, ClosedById=ih.AccountId
                        FROM IncidentHistory ih
                        WHERE ih.IncidentId = CommonIncidentProgressTracking.IncidentId
                        AND ih.State = 3;

                        UPDATE CommonIncidentProgressTracking
                        SET ReOpenCount=ReOpenCount+1
                        FROM IncidentHistory ih
                        WHERE ih.IncidentId = CommonIncidentProgressTracking.IncidentId
                        AND ih.State = 4;";
                cmd.ExecuteNonQuery();
            }

            _configuration.Value.HaveFilledDatabase = true;
            _configuration.Save();
        }
    }
}
