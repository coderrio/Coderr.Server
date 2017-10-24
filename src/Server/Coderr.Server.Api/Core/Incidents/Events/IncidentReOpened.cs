using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Events
{
    /// <summary>
    ///     Our user had closed the incident and we just got a new report despite that.
    /// </summary>
    [Message]
    public class IncidentReOpened
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentReOpened" />.
        /// </summary>
        /// <param name="applicationId">application that the incident belongs to</param>
        /// <param name="incidentId">incident id</param>
        /// <param name="createdAtUtc">when the new report was created (in the client library)</param>
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
        ///     when the new report was created (in the client library)
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Incident that the received report belongs to.
        /// </summary>
        public int IncidentId { get; set; }
    }
}