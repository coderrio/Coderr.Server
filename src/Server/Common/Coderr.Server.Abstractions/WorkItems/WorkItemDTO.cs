using System;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    ///     Work item linked to an incident.
    /// </summary>
    public class WorkItemDTO
    {
        public WorkItemDTO(int applicationId, int incidentId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            ApplicationId = applicationId;
            IncidentId = incidentId;
        }

        protected WorkItemDTO()
        {

        }

        /// <summary>
        ///     Url used to fetch updates
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Specified by the work item service when the item is created.
        ///     </para>
        /// </remarks>
        public string ApiUrl { get; set; }

        public int ApplicationId { get; private set; }
        public int IncidentId { get; private set; }

        /// <summary>
        ///     "Jira", "AzureDevOps"
        /// </summary>
        public string IntegrationName { get; set; }

        public string ReproduceSteps { get; set; }


        /// <summary>
        ///     State in the other system.
        /// </summary>
        public WorkItemState State { get; set; }

        public string SystemInformation { get; set; }

        public string[] Tags { get; set; }

        /// <summary>
        ///     Display name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Url that the user can click on to get to the other system.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Specified by the work item service when the item is created.
        ///     </para>
        /// </remarks>
        public string UiUrl { get; set; }

        /// <summary>
        ///     Id in the other system
        /// </summary>
        public string WorkItemId { get; set; }
    }
}