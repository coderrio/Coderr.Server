using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Queries;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using DotNetCqs;
using Microsoft.TeamFoundation.Core.WebApi;
using log4net;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Queries
{
    internal class GetProjectsHandler : IQueryHandler<GetProjects, GetProjectsResult>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(GetProjectsHandler));

        public async Task<GetProjectsResult> HandleAsync(IMessageContext context, GetProjects query)
        {
            var client = new SettingsClient(query.PersonalAccessToken, query.Url);
            try
            {
                var projects = await client.ListProjects();
                var items = new List<GetProjectsResultItem>();
                foreach (var project in projects)
                {
                    if (project.State != ProjectState.New
                        && project.State != ProjectState.Unchanged
                        && project.State != ProjectState.WellFormed)
                        continue;

                    var item = new GetProjectsResultItem { Id = project.Id.ToString(), Name = project.Name };
                    items.Add(item);
                }
    
                return new GetProjectsResult { Items = items.OrderBy(x => x.Name).ToArray() };
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to load projects", ex);
                return new GetProjectsResult { Items = new GetProjectsResultItem[0] };
            }
        }
    }
}