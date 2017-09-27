namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Action to take when a filter is apssed
    /// </summary>
    public enum TriggerRuleAction
    {
        /// <summary>
        ///     Do not execute the trigger
        /// </summary>
        AbortTrigger,

        /// <summary>
        ///     Middle manager principle: Lets delegate the decision to the next rule.
        /// </summary>
        ContinueWithNextRule,

        /// <summary>
        ///     Do not check any more rules, just execute the god damn trigger.
        /// </summary>
        ExecuteActions
    }
}