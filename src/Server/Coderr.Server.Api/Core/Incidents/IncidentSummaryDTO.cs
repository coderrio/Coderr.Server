using System;

namespace codeRR.Server.Api.Core.Incidents
{
    /// <summary>
    ///     A small summary of an incident, typically used to list incidents.
    /// </summary>
    public class IncidentSummaryDTO
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentSummaryDTO" />.
        /// </summary>
        /// <param name="id">incident id</param>
        /// <param name="name">incident name</param>
        /// <exception cref="ArgumentNullException">name</exception>
        /// <exception cref="ArgumentOutOfRangeException">incident id</exception>
        public IncidentSummaryDTO(int id, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (id <= 0) throw new ArgumentOutOfRangeException("id");

            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected IncidentSummaryDTO()
        {
        }

        /// <summary>
        ///     Application that the incident belongs to
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of that application
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     When the incident was created (when we received the first report).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Incident was closed but then received a new error report.
        /// </summary>
        public bool IsReOpened { get; set; }

        /// <summary>
        /// someone is assigned to this incident
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        ///     Update is both when the incident was open/closed and when we received a new report. TODO: Should be refactored into
        ///     two fields.
        /// </summary>
        public DateTime LastUpdateAtUtc { get; set; }

        /// <summary>
        ///     Incident name (typically first line of the exception message)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Number of reports that we've received. Should be the total amount (including those that have been deleted due to
        ///     retention days).
        /// </summary>
        public int ReportCount { get; set; }
    }
}