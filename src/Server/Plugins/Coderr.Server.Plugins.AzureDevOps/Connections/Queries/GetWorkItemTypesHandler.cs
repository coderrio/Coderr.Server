using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    class GetWorkItemTypesHandler : IQueryHandler<GetWorkItemTypes, GetWorkItemTypesResult>
    {
        public async Task<GetWorkItemTypesResult> HandleAsync(IMessageContext context, GetWorkItemTypes query)
        {
            var client = new SettingsClient(query.PersonalAccessToken, query.Url);
            var result = await client.GetWorkItemTypes(query.ProjectNameOrId);
            return new GetWorkItemTypesResult
            {
                Items = result
                    .OrderBy(x => x.Value)
                    .Select(x => new GetWorkItemTypesResultItem { ReferenceName = x.Key, Name = x.Value })
                    .ToArray()
            };
        }
    }
}
