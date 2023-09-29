using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    internal class GetAreaPathsHandler : IQueryHandler<GetAreaPaths, GetAreaPathsResult>
    {
        public async Task<GetAreaPathsResult> HandleAsync(IMessageContext context, GetAreaPaths query)
        {
            var client = new SettingsClient(query.PersonalAccessToken, query.Url);
            var rootArea = await client.GetAreas(query.ProjectNameOrId);

            var items = new List<GetAreaPathsResultItem>();
            ConvertItem(rootArea, items);
            return new GetAreaPathsResult { Items = items.OrderBy(x => x.Path).ToArray() };
        }

        private void ConvertItem(ClassificationNode area, List<GetAreaPathsResultItem> items)
        {
            items.Add(new GetAreaPathsResultItem { Id = area.Id, Name = area.Name, Path = area.Path });

            foreach (var node in area.Children) ConvertItem(node, items);
        }
    }
}