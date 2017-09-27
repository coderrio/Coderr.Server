using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Result for <see cref="ITriggerRule" />.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterResult")]
    public enum FilterResult
    {
        /// <summary>
        ///     Rule did not match the given conditions.
        /// </summary>
        NotMatched,

        /// <summary>
        ///     Stop processing other rules and grant this report
        /// </summary>
        Grant,

        /// <summary>
        ///     Stop process other rules and revoke this report
        /// </summary>
        Revoke,

        /// <summary>
        ///     Ok by us, pass to any other roles.
        /// </summary>
        Continue
    }
}