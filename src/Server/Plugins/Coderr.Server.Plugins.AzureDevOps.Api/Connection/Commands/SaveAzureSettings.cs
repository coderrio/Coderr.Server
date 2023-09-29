using Coderr.Server.Api;

namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Commands
{
    [Command]
    public class SaveAzureSettings
    {
        public int ApplicationId { get; set; }
        public string AreaPath { get; set; }
        public string AreaPathId { get; set; }

        /// <summary>
        ///     Started working on the item
        /// </summary>
        public string AssignedStateName { get; set; }

        public bool AutoAddCritical { get; set; }
        public bool AutoAddImportant { get; set; }

        /// <summary>
        ///     Closed can also mean that it wont be fixed.
        /// </summary>
        public string ClosedStateName { get; set; }

        public string PersonalAccessToken { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }

        /// <summary>
        ///     Name of the state that is used when the item is closed.
        /// </summary>
        public string SolvedStateName { get; set; }
        
        public string Url { get; set; }

        /// <summary>
        ///     Type of work item that we are integrating against in the project.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used when we create a new work item for issues.
        ///     </para>
        /// </remarks>
        public string WorkItemTypeName { get; set; }
    }
}