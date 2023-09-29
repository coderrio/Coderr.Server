using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Client;
using log4net;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public class SettingsClient : AzureDevOpsClient
    {
        private ILog _logger = LogManager.GetLogger(typeof(SettingsClient));
        private static bool IsGraphEnabled = true;

        public SettingsClient(string personalAccessToken, string url) : base(personalAccessToken, url)
        {
        }

        public async Task<ClassificationNode> GetAreas(string projectName)
        {
            var client = CreateWitClient();


            var rootArea = await client.GetClassificationNodeAsync(projectName, TreeStructureGroup.Areas, null, 100);
            return Convert(rootArea);
        }

        public async Task<ClassificationNode> GetIterationPaths(string projectName)
        {
            var client = CreateWitClient();

            var rootArea =
                await client.GetClassificationNodeAsync(projectName, TreeStructureGroup.Iterations, null, 100);
            return Convert(rootArea);
        }

        public async Task<IEnumerable<TeamProjectCollectionReference>> ListCompanies()
        {
            var connection = CreateConnection();
            var client = connection.GetClient<ProjectCollectionHttpClient>();
            return await client.GetProjectCollections();
        }

        /// <summary>
        /// List all projects.
        /// </summary>
        /// <returns></returns>
        public async Task<IPagedList<TeamProjectReference>> ListProjects()
        {
            var connection = CreateConnection();
            var client = connection.GetClient<ProjectHttpClient>();
            return await client.GetProjects(ProjectState.All);
        }

        public async Task<IReadOnlyList<string>> GetTypeFields(string projectId, string workItemTypeName)
        {
            var connection = CreateConnection();

            var id = Guid.Parse(projectId);
            //var props = await projectClient.GetProjectPropertiesAsync(id, new[] {"System.Process Template"});
            //var templateType = props[0].Value.ToString();
            var witClient = CreateWitClient(connection);
            var fields = await witClient.GetWorkItemTypeStatesAsync(id, workItemTypeName, WorkItemTypeFieldsExpandLevel.AllowedValues);
            return fields.Select(x => x.Name).ToList();
        }

        public async Task<IDictionary<string, string>> GetWorkItemTypes(string projectId)
        {
            var connection = CreateConnection();

            var id = Guid.Parse(projectId);
            //var props = await projectClient.GetProjectPropertiesAsync(id, new[] {"System.Process Template"});
            //var templateType = props[0].Value.ToString();
            var witClient = CreateWitClient(connection);
            var types = await witClient.GetWorkItemTypesAsync(id);
            return types.ToDictionary(x => x.ReferenceName, x => x.Name);
        }

        //public async Task<string> GetEmailFrom(string descriptor)
        //{
        //    //GET https://vssps.dev.azure.com/{organization}/_apis/graph/users?
        //    var connection = CreateConnection();
        //    var client = connection.GetClient<GraphHttpClient>();
        //    var user = await client.GetUserAsync(descriptor);
        //    //user.MailAddress

        //    var clietn2 = connection.GetClient<UserHttpClient>();
        //    clietn2.GetUserAsync("").Result.Mail
        //    clietn2.ReadIdentityAsync(Guid.Empty).Result.Properties
        //    var user2 = await clietn2.GetDescriptorByIdAsync(Guid.Empty);
        //    user2.
        //}


        private ClassificationNode Convert(WorkItemClassificationNode dtoArea)
        {
            var area = new ClassificationNode
            {
                Id = dtoArea.Id,
                Name = dtoArea.Name,

                // I don't know why MS adds "\Area" to the path, but Azure DevOps
                // refuses to create new work items with that in the name.
                Path = dtoArea.Path.Replace("\\Area", ""),

                Children = new List<ClassificationNode>()
            };

            if (dtoArea.HasChildren != true)
                return area;

            foreach (var child in dtoArea.Children)
            {
                var c = Convert(child);
                area.Children.Add(c);
            }

            return area;
        }

        public async Task<string> GetEmailAddress(SubjectDescriptor descriptor)
        {
            if (!IsGraphEnabled)
                return null;

            // This works in Azure Cloud, but not premise
            try
            {
                var connection = CreateConnection();
                var client = connection.GetClient<GraphHttpClient>();
                var user = await client.GetUserAsync(descriptor.ToString());
                return user?.MailAddress;
            }
            catch (VssResourceNotFoundException ex)
            {
                IsGraphEnabled = false;
                Err.Report(ex, new {userDescriptor = descriptor.ToString()});
                _logger.Error($"Graph API is not supported, cannot lookup {descriptor} [{ex.Message}]");
                return null;
            }
        }
    }
}