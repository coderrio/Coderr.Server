using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    internal class GetAzureSettingsHandler : IQueryHandler<GetAzureSettings, GetAzureSettingsResult>
    {
        private readonly ISettingsRepository _repository;

        public GetAzureSettingsHandler(ISettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAzureSettingsResult> HandleAsync(IMessageContext context, GetAzureSettings query)
        {
            var entity = await _repository.Get(query.ApplicationId);
            if (entity == null)
                return null;

            return new GetAzureSettingsResult
            {
                ApplicationId = entity.ApplicationId,
                AreaPath = entity.AreaPath,
                AreaPathId = entity.AreaPathId,
                PersonalAccessToken = entity.PersonalAccessToken,
                ProjectId = entity.ProjectId,
                ProjectName = entity.ProjectName,
                Url = entity.Url,
                AutoAddCritical = entity.AutoAddCritical,
                AutoAddImportant = entity.AutoAddImportant,
                AssignedStateName = entity.AssignedStateName,
                ClosedStateName = entity.ClosedStateName,
                SolvedStateName = entity.SolvedStateName,
                //StateFieldName = entity.StateFieldName,
                WorkItemTypeName = entity.WorkItemTypeName
            };
        }
    }
}