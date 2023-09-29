using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Common.AzureDevOps.App.Connections
{
    public class Settings
    {
        public int ApplicationId { get; set; }

        /// <summary>
        /// Account id
        /// </summary>
        /// <remarks>
        ///<para>
        ///Used when synchronizing work items when we cant match the integrated systems user with our user (and the item have changed state)
        /// </para>
        /// </remarks>
        public int CreatedById { get; set; }
        public string Url { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string AreaPath { get; set; }
        public string AreaPathId { get; set; }
        public string PersonalAccessToken { get; set; }

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

        /// <summary>
        /// Add errors escalated to critical into the backlog.
        /// </summary>
        public bool AutoAddCritical { get; set; }
        /// <summary>
        /// Add errors escalated to important into the backlog.
        /// </summary>
        public bool AutoAddImportant { get; set; }

    }
}
