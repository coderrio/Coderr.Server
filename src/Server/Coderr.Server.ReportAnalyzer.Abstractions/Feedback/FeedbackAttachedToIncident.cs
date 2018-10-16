namespace Coderr.Server.ReportAnalyzer.Abstractions.Feedback
{
    /// <summary>
    ///     Feedback was attached to incident.
    /// </summary>
    public class FeedbackAttachedToIncident
    {
        /// <summary>
        ///     Incident that the feedback was attached to.
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Feedback message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Email address to the user that wrote the message (optional)
        /// </summary>
        public string UserEmailAddress { get; set; }
    }
}