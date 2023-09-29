using System.Collections.Generic;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    /// Used to find our users in the external system.
    /// </summary>
    public class WorkItemUserMapping
    {
        public int AccountId { get; set; }

        /// <summary>
        /// Id in the external system.
        /// </summary>
        /// <remarks>
        ///<para>This must contain the value that is used in the work item.</para>
        /// </remarks>
        public string ExternalId { get; set; }
        public IDictionary<string, string> AdditionalData { get; set; }
    }
}
