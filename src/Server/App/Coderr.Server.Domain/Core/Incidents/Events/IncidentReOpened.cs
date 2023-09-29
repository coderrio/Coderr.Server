using System;

namespace Coderr.Server.Domain.Core.Incidents.Events
{
    /// <summary>
    ///     Our user had closed the incident and we just got a new report despite that.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used both in the ReportAnalyzer and in the Application.
    ///     </para>
    /// </remarks>
    public class IncidentReOpened
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentReOpened" />.
        /// </summary>
        /// <param name="applicationId">application that the incident belongs to</param>
        /// <param name="incidentId">incident id</param>
        /// <param name="createdAtUtc">when the new error report was created (in the client library)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IncidentReOpened(int applicationId, int incidentId, DateTime createdAtUtc)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");

            ApplicationId = applicationId;
            IncidentId = incidentId;
            CreatedAtUtc = createdAtUtc;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected IncidentReOpened()
        {
        }

        /// <summary>
        ///     Application that the report belongs to.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Version that we reopened the incident in.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        ///     when the new error report was created in the client library. I.e. the report that triggered the reopening of the incident.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Incident that the received report belongs to.
        /// </summary>
        public int IncidentId { get; set; }
    }
}