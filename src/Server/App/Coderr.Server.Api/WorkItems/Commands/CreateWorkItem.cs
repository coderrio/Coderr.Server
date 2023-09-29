using System;

namespace Coderr.Server.Api.WorkItems.Commands
{
    /// <summary>
    ///     Create a new work item.
    /// </summary>
    [Command]
    public class CreateWorkItem
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CreateWorkItem" /> class.
        /// </summary>
        /// <param name="applicationId">Application that the incident belongs to.</param>
        /// <param name="incidentId">Incident to create an work item for.</param>
        public CreateWorkItem(int applicationId, int incidentId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            ApplicationId = applicationId;
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected CreateWorkItem()
        {
        }

        /// <summary>
        /// Application that the incident belongs to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        /// Incident to create an work item for.
        /// </summary>
        public int IncidentId { get; private set; }
    }
}