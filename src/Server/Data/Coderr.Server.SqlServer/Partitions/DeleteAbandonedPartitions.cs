using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Griffin.ApplicationServices;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Partitions
{
    [ContainerService(RegisterAsSelf = true)]
    internal class DeleteAbandonedPartitions : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private static DateTime _executionDate = DateTime.Today;

        public DeleteAbandonedPartitions(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            if (_executionDate == DateTime.Today)
                return;

            _executionDate = DateTime.Today;
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM IncidentPartitionValues
                                    FROM IncidentPartitionValues WITH(ReadUncommitted)
                                    LEFT JOIN Incidents WITH(ReadUncommitted) ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM InboundPartitionValues
                                    FROM InboundPartitionValues WITH(ReadUncommitted)
                                    LEFT JOIN Incidents WITH(ReadUncommitted) ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}