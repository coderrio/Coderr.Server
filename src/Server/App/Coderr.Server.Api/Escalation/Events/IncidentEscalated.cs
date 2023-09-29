using System;

namespace Coderr.Server.Api.Escalation.Events
{
    /// <summary>
    ///     Incident has been escalated.
    /// </summary>
    public class IncidentEscalated
    {
        public IncidentEscalated(int applicationId, int incidentId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            ApplicationId = applicationId;
            IncidentId = incidentId;
        }

        protected IncidentEscalated()
        {

        }

        /// <summary>
        ///     Application that the incident belongs to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Incident being escalated.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Error has been escalated to critical.
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        ///     Error has been escalated to important.
        /// </summary>
        public bool IsImportant { get; set; }
    }
}