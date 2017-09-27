namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Make a decision based on exception information
    /// </summary>
    public class TriggerExceptionRule : TriggerRuleBase
    {
        /// <summary>
        ///     Field in the exception details that should be inspected (property name from the Exception class)
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///     Value that should be matched.
        /// </summary>
        public string Value { get; set; }
    }
}