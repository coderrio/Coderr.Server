namespace Coderr.Server.App.Modules.Triggers.Rules
{
    /// <summary>
    ///     Base for trigger rules
    /// </summary>
    public class RuleBase
    {
        /// <summary>
        ///     How to compare the values
        /// </summary>
        public FilterCondition Condition { get; set; }

        /// <summary>
        ///     Result to use if value comparison succeeds.
        /// </summary>
        public FilterResult ResultToUse { get; set; }
    }
}