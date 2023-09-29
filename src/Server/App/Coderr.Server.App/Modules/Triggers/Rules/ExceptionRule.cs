namespace Coderr.Server.App.Modules.Triggers.Rules
{
    /// <summary>
    ///     Uses exception details (like Name, Message, StackTrace) to filter the trigger.
    /// </summary>
    public class ExceptionRule : RuleBase
    {
        /// <summary>
        ///     Exception field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///     Value to compare with
        /// </summary>
        public string Value { get; set; }
    }
}