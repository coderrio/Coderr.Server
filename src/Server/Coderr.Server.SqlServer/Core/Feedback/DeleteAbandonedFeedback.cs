using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Core.Feedback
{
    [ContainerService(RegisterAsSelf = true)]
    internal class DeleteAbandonedFeedback : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteAbandonedFeedback(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM IncidentFeedback
                                    FROM IncidentFeedback
                                    LEFT JOIN Incidents ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}