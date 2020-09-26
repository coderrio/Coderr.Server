using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Core.Incidents
{
    [ContainerService(RegisterAsSelf = true)]
    internal class DeleteAbandonedTags : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteAbandonedTags(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM IncidentTags
                                    FROM IncidentTags
                                    LEFT JOIN Incidents ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}