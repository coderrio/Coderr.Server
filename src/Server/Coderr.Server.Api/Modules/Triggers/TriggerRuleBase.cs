namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Base class for rules.
    /// </summary>
    public class TriggerRuleBase
    {
        /// <summary>
        ///     Filter that should be passed
        /// </summary>
        public TriggerFilterCondition Filter { get; set; }

        /// <summary>
        ///     Did we pass the filter? Then do this.
        /// </summary>
        public TriggerRuleAction ResultToUse { get; set; }
    }
}