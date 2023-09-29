using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    class GetWorkItemStatesHandler : IQueryHandler<GetWorkItemStates, GetWorkItemStatesResult>
    {
        public async Task<GetWorkItemStatesResult> HandleAsync(IMessageContext context, GetWorkItemStates query)
        {
            var client = new SettingsClient(query.PersonalAccessToken, query.Url);
            var types = await client.GetTypeFields(query.ProjectId, query.WorkItemTypeName);

            var items = new List<GetWorkItemTypesResultItem>();
            return new GetWorkItemStatesResult
            {
                Items = types
                    .OrderBy(x => x)
                    .Select(x => new GetWorkItemStatesResultItem { Name = x })
                    .ToArray()
            };
        }
    }
}