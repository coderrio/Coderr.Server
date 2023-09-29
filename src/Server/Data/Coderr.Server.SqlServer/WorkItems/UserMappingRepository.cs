using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.WorkItems;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.WorkItems
{
    [ContainerService]
    internal class UserMappingRepository : IUserMappingRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public UserMappingRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task Create(WorkItemUserMapping mapping)
        {
            return _unitOfWork.InsertAsync(mapping);
        }

        public Task<WorkItemUserMapping> Get(int accountId)
        {
            return _unitOfWork.FirstOrDefaultAsync<WorkItemUserMapping>("accountId = @accountId", new {accountId});
        }

        public Task<WorkItemUserMapping> GetByExternalId(string externalId)
        {
            return _unitOfWork.FirstOrDefaultAsync<WorkItemUserMapping>("externalId = @externalId", new {externalId});
        }
    }
}