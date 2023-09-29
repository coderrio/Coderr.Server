using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    class GetIterationsHandler : IQueryHandler<GetIterations, GetIterationsResult>
    {
        public async Task<GetIterationsResult> HandleAsync(IMessageContext context, GetIterations query)
        {
            var client = new SettingsClient(query.PersonalAccessToken, query.Url);
            var rootArea = await client.GetIterationPaths(query.ProjectNameOrId);

            var items = new List<GetIterationsResultItem>();
            ConvertItem(rootArea, items);
            return new GetIterationsResult { Items = items.ToArray() };
        }

        private void ConvertItem(ClassificationNode area, ICollection<GetIterationsResultItem> items)
        {
            items.Add(new GetIterationsResultItem { Id = area.Id, Name = area.Name, Path = area.Path });

            foreach (var node in area.Children) ConvertItem(node, items);
        }
    }
}
