namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Result for <see cref="GetIncidentForClosePage" />.
    /// </summary>
    public class GetIncidentForClosePageResult
    {
        /// <summary>
        ///     A summary of the incident
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Number of update subscribers (i.e. users that want status updates).
        /// </summary>
        public int SubscriberCount { get; set; }
    }
}