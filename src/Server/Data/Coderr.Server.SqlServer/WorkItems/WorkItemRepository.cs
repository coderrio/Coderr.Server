using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.App.WorkItems;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.WorkItems
{
    [ContainerService]
    public class WorkItemRepository : IWorkItemRepository
    {
        private IAdoNetUnitOfWork _unitOfWork;

        public WorkItemRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkItemMapping> Find(int incidentId)
        {
            return await _unitOfWork.FirstOrDefaultAsync<WorkItemMapping>(new { IncidentId = incidentId });
        }

        public async Task<IReadOnlyList<WorkItemMapping>> FindAllWorkItems(string integrationName)
        {
            return await _unitOfWork.ToListAsync<WorkItemMapping>("IntegrationName = @integrationName", new { integrationName });
        }

        public async Task Create(WorkItemMapping item)
        {
            await _unitOfWork.InsertAsync(item);
        }

        public async Task MapApplication(int applicationId, string integrationName)
        {
            if (await _unitOfWork.FirstOrDefaultAsync<WorkItemIntegrationMapping>("ApplicationId = @id", new { id = applicationId }) == null)
                await _unitOfWork.InsertAsync(new WorkItemIntegrationMapping
                {
                    IntegrationName = integrationName,
                    ApplicationId = applicationId
                });
            else
            {
                await _unitOfWork.UpdateAsync(new WorkItemIntegrationMapping
                {
                    IntegrationName = integrationName,
                    ApplicationId = applicationId
                });
            }
        }

        public async Task<string> GetIntegrationName(int applicationId)
        {
            var entity = await _unitOfWork.FirstOrDefaultAsync<WorkItemIntegrationMapping>("ApplicationId = @applicationId", new { applicationId });
            return entity?.IntegrationName;
        }

        public async Task DeleteAbandonedWorkItems()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"DELETE FROM WorkItemMapping
                                    FROM WorkItemMapping
                                    LEFT JOIN Incidents ON (Incidents.Id = IncidentId)
                                    WHERE Incidents.Id IS NULL";
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(WorkItemMapping workItem)
        {
            await _unitOfWork.DeleteAsync<WorkItemMapping>(new {workItem.IncidentId});
        }
    }
}
