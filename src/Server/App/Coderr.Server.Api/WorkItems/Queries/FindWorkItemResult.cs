namespace Coderr.Server.Api.WorkItems.Queries
{
    /// <summary>
    /// Result for <see cref="FindWorkItem"/>.
    /// </summary>
    public class FindWorkItemResult
    {
        /// <summary>
        /// Id in the system that we integrate against.
        /// </summary>
        public string WorkItemId { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url to the work item in the integrated system.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Application that the incident belongs to.
        /// </summary>
        public int ApplicationId { get; set; }
    }
}