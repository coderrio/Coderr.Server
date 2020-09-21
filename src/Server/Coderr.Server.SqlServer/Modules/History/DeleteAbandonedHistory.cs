using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Modules.History
{
    [ContainerService(RegisterAsSelf = true)]
    internal class DeleteAbandonedHistory : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteAbandonedHistory(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM IncidentHistory
                                    FROM IncidentHistory
                                    LEFT JOIN Incidents ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}