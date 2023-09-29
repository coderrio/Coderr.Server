using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Common.AzureDevOps.App.Connections;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Azure.DevOps
{
    [ContainerService]
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public SettingsRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Settings> Get(int applicationId)
        {
            return await _unitOfWork.FirstOrDefaultAsync<Settings>(new {ApplicationId = applicationId});
        }

        public async Task<List<Settings>> GetAll()
        {
            return await _unitOfWork.ToListAsync<Settings>("SELECT * From AzureDevOpsSettings");
        }

        public async Task Save(Settings settings)
        {
            if (settings.WorkItemTypeName == "")
                settings.WorkItemTypeName = null;

            var dbEntity = await Get(settings.ApplicationId);
            if (dbEntity == null)
                await _unitOfWork.InsertAsync(settings);
            else
                await _unitOfWork.UpdateAsync(settings);
        }
    }
}