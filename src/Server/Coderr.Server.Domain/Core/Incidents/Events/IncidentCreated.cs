using System;

namespace Coderr.Server.Domain.Core.Incidents.Events
{
    /// <summary>
    ///     Event for the domain (and not the report analyzer).
    /// </summary>
    public class IncidentCreated
    {
        public IncidentCreated(int applicationId, int incidentId, string incidentName, string exceptionTypeName)
        {
            if (incidentName == null) throw new ArgumentNullException(nameof(incidentName));
            if (exceptionTypeName == null) throw new ArgumentNullException(nameof(exceptionTypeName));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
            IncidentId = incidentId;

            var pos = incidentName.IndexOfAny(new[] { '\r', '\n' });
            if (pos != -1)
                incidentName = incidentName.Substring(0, pos);
            IncidentName = incidentName;
            ExceptionTypeName = exceptionTypeName;
        }

        protected IncidentCreated()
        {
        }

        /// <summary>
        ///     Application that the incident is for
        /// </summary>

        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Version (if reported by the client)
        /// </summary>
        public string ApplicationVersion { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Full name of the exception type
        /// </summary>
        public string ExceptionTypeName { get; private set; }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Incident name
        /// </summary>
        public string IncidentName { get; private set; }
    }
}