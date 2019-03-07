using System;

namespace Coderr.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    ///     Close incident (i.e. we have corrected the issue)
    /// </summary>
    [Message]
    public class CloseIncident
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CloseIncident" />.
        /// </summary>
        /// <param name="solution">Markdown formatted string detailing how we solved this incident.</param>
        /// <param name="incidentId">Incident that was solved.</param>
        public CloseIncident(string solution, int incidentId)
        {
            if (solution == null) throw new ArgumentNullException("solution");
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
            Solution = solution;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected CloseIncident()
        {
        }

        /// <summary>
        ///     Which version that incident is solved in (like "1.2.1").
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        ///     Can send notifications to everyone which has reported exceptions through our system.
        /// </summary>
        public bool CanSendNotification { get; set; }

        /// <summary>
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Text to send as email body
        /// </summary>
        public string NotificationText { get; set; }

        /// <summary>
        ///     Title of outbound notification.
        /// </summary>
        public string NotificationTitle { get; set; }

        /// <summary>
        ///     If this solution can be shared with other OTE customers.
        /// </summary>
        public bool ShareSolution { get; set; }

        /// <summary>
        ///     How the incident has been fixed.
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        ///     User that closed the incident
        /// </summary>
        /// <remarks>
        ///     <para>Need to be named "UserId" so that the CQS mapper can add the logged in user id</para>
        /// </remarks>
        public int UserId { get; set; }

        /// <summary>
        /// When incident was closed (optional, "now" will be used when not specified)
        /// </summary>
        public DateTime? ClosedAtUtc { get; set; }
    }
}