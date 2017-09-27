using System.ComponentModel;

namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Filter condition
    /// </summary>
    public enum TriggerFilterCondition
    {
        /// <summary>
        ///     Inspected value should start with the filter string
        /// </summary>
        [Description("Starts with")] StartsWith,

        /// <summary>
        ///     Inspected value should end with the filter string
        /// </summary>
        [Description("Ends with")] EndsWith,

        /// <summary>
        ///     Inspected value should contain the filter string
        /// </summary>
        [Description("Contain")] Contains,

        /// <summary>
        ///     Inspected value should not contain the filter string
        /// </summary>
        [Description("Do not contain")] DoNotContain,

        /// <summary>
        ///     Inspected value should equal the filter string (case insensitive)
        /// </summary>
        [Description("Equals")] Equals
    }
}