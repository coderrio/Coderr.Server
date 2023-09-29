using System;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    ///     Keeps a link between our incident and the work item in the external system
    /// </summary>
    public class WorkItemMapping
    {
        public WorkItemMapping(int applicationId, int incidentId, string integrationName)
        {
            ApplicationId = applicationId;
            IncidentId = incidentId;
            IntegrationName = integrationName;
        }

        protected WorkItemMapping()
        {
        }

        public int ApplicationId { get; private set; }

        public int IncidentId { get; private set; }

        /// <summary>
        ///     When we synchronized the last time.
        /// </summary>
        public DateTime LastSynchronizationAtUtc { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///     Url that the user can click on to get to the other system.
        /// </summary>
        public string UiUrl { get; set; }

        /// <summary>
        /// Url used to fetch updates.
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        ///     Id in the other system
        /// </summary>
        public string WorkItemId { get; set; }

        public string IntegrationName { get; private set; }
    }
}