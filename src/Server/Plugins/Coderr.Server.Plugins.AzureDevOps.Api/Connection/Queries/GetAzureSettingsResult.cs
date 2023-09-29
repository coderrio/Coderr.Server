namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    public class GetAzureSettingsResult
    {
        public int ApplicationId { get; set; }
        public string AreaPath { get; set; }
        public string AreaPathId { get; set; }
        public string PersonalAccessToken { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Url { get; set; }
        public bool AutoAddCritical { get; set; }
        public bool AutoAddImportant { get; set; }


        /// <summary>
        /// Type of work item that we are integrating against in the project.
        /// </summary>
        public string WorkItemTypeName { get; set; }

        /// <summary>
        /// Started working on the item
        /// </summary>
        public string AssignedStateName { get; set; }

        /// <summary>
        /// Name of the state that is used when the item is closed.
        /// </summary>
        public string SolvedStateName { get; set; }

        /// <summary>
        /// Closed can also mean that it wont be fixed.
        /// </summary>
        public string ClosedStateName { get; set; }
    }
}