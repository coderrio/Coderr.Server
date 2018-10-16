namespace Coderr.Server.App.Modules.Triggers.Rules
{
    /// <summary>
    ///     Check a context collection in the trigger
    /// </summary>
    public class ContextCollectionRule : RuleBase
    {
        /// <summary>
        ///     Context collection to check
        /// </summary>
        public string ContextName { get; set; }


        /// <summary>
        ///     Property in that collection
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        ///     Value for the property
        /// </summary>
        public string PropertyValue { get; set; }
    }
}