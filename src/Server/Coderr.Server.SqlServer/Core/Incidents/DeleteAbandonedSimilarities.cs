using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Core.Incidents
{
    [ContainerService(RegisterAsSelf = true)]
    internal class DeleteAbandonedCorrelations : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteAbandonedCorrelations(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM IncidentContextCollections
                                    FROM IncidentContextCollections
                                    LEFT JOIN Incidents ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}