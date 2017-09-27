using System.ComponentModel;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Specifies how the filter value should be compared with the actual property data.
    /// </summary>
    public enum FilterCondition
    {
        /// <summary>
        ///     Should start with the given value
        /// </summary>
        [Description("Starts with")] StartsWith,

        /// <summary>
        ///     Should end with the given value
        /// </summary>
        [Description("Ends with")] EndsWith,

        /// <summary>
        ///     Should contain the given value
        /// </summary>
        [Description("Contain")] Contains,

        /// <summary>
        ///     Should not contain the given value
        /// </summary>
        [Description("Do not contain")] DoNotContain,

        /// <summary>
        ///     Should equal the given value.
        /// </summary>
        [Description("Equals")] Equals
    }
}