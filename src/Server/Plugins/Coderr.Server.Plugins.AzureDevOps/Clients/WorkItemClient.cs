using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Common.AzureDevOps.App.Connections;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public class WorkItemClient : AzureDevOpsClient, IWorkItemClient
    {
        private readonly Settings _settings;

        public WorkItemClient(Settings settings) : base(settings.PersonalAccessToken, settings.Url)
        {
            _settings = settings;
        }

        /// <summary>
        ///     Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<Comment> AddComment(int workItemId, string comment)
        {
            var client = CreateWitClient();
            var k = new CommentCreate {Text = comment};
            var result = await client.AddCommentAsync(k, _settings.ProjectName, workItemId);
            return result;
        }

        /// <summary>
        ///     Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<WorkItem> Create(WorkItemDTO workItemDto, string areaPath)
        {
            var tags = workItemDto.Tags.Where(x => x != "important" && x != "critical").ToList();
            tags.Add("coderr");
            var patchDocument = new JsonPatchDocument
            {
                {Fields.Title, workItemDto.Title},
                {Fields.ReproduceSteps, workItemDto.ReproduceSteps.Replace("\r\n", "  <br>\r\n")},
                {Fields.SystemInfo, workItemDto.SystemInformation.Replace("\r\n", "  <br>\r\n")},
                {
                    // This field is used when repro/systeminfo is not visible (different process templates).
                    Fields.Description, workItemDto.ReproduceSteps.Replace("\r\n", "  <br>\r\n")
                                        + "  <br>\r\n  <br>\r\n# System information  <br>\r\n" +
                                        workItemDto.SystemInformation.Replace("\r\n", "  <br>\r\n")
                },
                //{Fields.Priority, "1"},
                //{Fields.Severity, "1 - Medium"},
                {Fields.AreaPath, areaPath},
                {Fields.Tags, string.Join(";", tags)}
            };


            var client = CreateWitClient();
            var result =
                await client.CreateWorkItemAsync(patchDocument, _settings.ProjectName, _settings.WorkItemTypeName);
            return result;
        }

        /// <summary>
        ///     Update a work item.
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<WorkItem> Update(int workItemId, WorkItemMapping workItem)
        {
            var patchDocument = new JsonPatchDocument
            {
                {Fields.Title, "MyBug"},
                {
                    Fields.ReproduceSteps,
                    "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http:// msdn.microsoft.com/library/live/hh826547.aspx"
                },
                //{Fields.Priority, "1"},
                //{Fields.Severity, "2 - High"},
                {Fields.History, "Added by Coderr (https://coderr.io)"},
                {Fields.Tags, "coderr;backend"}
            };

            var client = CreateWitClient();

            var result =
                await client.CreateWorkItemAsync(patchDocument, _settings.ProjectName, _settings.WorkItemTypeName);
            return result;
        }

        /// <summary>
        ///     Get a work item
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<WorkItem> Get(int workItemId)
        {
            //var fields = new[]
            //{
            //    Fields.Id, Fields.State, Fields.StateChangeDate, Fields.ClosedBy, Fields.ClosedDate,
            //    Fields.ResolvedBy, Fields.ResolvedReason, Fields.ResolvedDate, Fields.AssignedTo,
            //    Fields.StateChangeDate, Fields.ActivatedBy, Fields.ActivatedDate
            //};

            try
            {
                var client = CreateWitClient();
                var result = await client.GetWorkItemAsync(workItemId);
                return result;
            }
            catch (VssServiceException ex)
            {
                // do not exist.
                if (ex.Message.Contains("TF401232"))
                    return null;
                throw;
            }
        }

        public async Task Assign(int workItemId, string emailAddress)
        {
            var patchDocument = new JsonPatchDocument
            {
                {Fields.AssignedTo, emailAddress}, {Fields.State, _settings.AssignedStateName}
            };

            var client = CreateWitClient();
            await client.UpdateWorkItemAsync(patchDocument, workItemId);
        }

        public async Task Solve(int workItemId, string emailAddress, string solution, string version)
        {
            var patchDocument = new JsonPatchDocument
            {
                {Fields.ClosedBy, emailAddress},
                {Fields.State, _settings.SolvedStateName ?? _settings.ClosedStateName}
            };

            var client = CreateWitClient();
            await client.UpdateWorkItemAsync(patchDocument, workItemId);

            var comment = "";
            if (!string.IsNullOrEmpty(solution))
                comment = "Solution: " + solution + "\r\n";
            if (!string.IsNullOrEmpty(version))
                comment = "Corrected in: " + version + "\r\n";

            if (comment != "")
                await client.AddCommentAsync(new CommentCreate {Text = comment}, _settings.ProjectName, workItemId);
        }

        public async Task<IReadOnlyList<IdentityRef>> GetMembers()
        {
            var con = CreateConnection();
            var client = con.GetClient<TeamHttpClient>();
            var teams = await client.GetTeamsAsync(_settings.ProjectId);
            var allMembers = new Dictionary<string, IdentityRef>();
            foreach (var team in teams)
            {
                var members =
                    await client.GetTeamMembersWithExtendedPropertiesAsync(_settings.ProjectId, team.Id.ToString());
                foreach (var member in members) allMembers[member.Identity.Id] = member.Identity;
            }

            return allMembers.Values.ToList();
        }
    }
}