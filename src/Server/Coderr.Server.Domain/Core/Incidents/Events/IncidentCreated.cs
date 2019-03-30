using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Domain.Core.Incidents.Events
{
    /// <summary>
    /// Event for the domain (and not the report analyzer).
    /// </summary>
    public class IncidentCreated
    {
        public IncidentCreated(int applicationId, int incidentId, string incidentDescription, string exceptionTypeName)
        {
            if (incidentDescription == null) throw new ArgumentNullException(nameof(incidentDescription));
            if (exceptionTypeName == null) throw new ArgumentNullException(nameof(exceptionTypeName));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
            IncidentId = incidentId;

            var pos = incidentDescription.IndexOfAny(new[] {'\r', '\n'});
            if (pos != -1)
                incidentDescription = incidentDescription.Substring(0, pos);
            IncidentName = incidentDescription;
            ExceptionTypeName = exceptionTypeName;
        }

        protected IncidentCreated()
        {

        }

        /// <summary>
        /// Incident id
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        /// Application that the incident is for
        /// </summary>

        public int ApplicationId { get; private set; }

        /// <summary>
        /// Incident name
        /// </summary>
        public string IncidentName { get; private set; }

        /// <summary>
        /// Full name of the exception type
        /// </summary>
        public string ExceptionTypeName { get; private set; }

        /// <summary>
        /// Version (if reported by the client)
        /// </summary>
        public string ApplicationVersion { get; set; }
    }
}
