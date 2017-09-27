namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Context when doing the filtering
    /// </summary>
    public class TriggerContextRule : TriggerRuleBase
    {
        /// <summary>
        ///     Context name currently being inspected
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        ///     Property in that context
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///     Value
        /// </summary>
        public string PropertyValue { get; set; }
    }
}