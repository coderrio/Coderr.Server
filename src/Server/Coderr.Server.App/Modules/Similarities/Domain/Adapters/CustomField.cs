using System;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Custom fields are added by the value adapters.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For instance the UserAgent string can be parsed into different custom fields like OS, Browser, OS etc.
    ///     </para>
    /// </remarks>
    public class CustomField
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CustomField" />.
        /// </summary>
        /// <param name="contextName">Context collection that the new field belongs in.</param>
        /// <param name="propertyName">Property name, like "BrowserVersion"</param>
        /// <param name="value">Actual value</param>
        public CustomField(string contextName, string propertyName, object value)
        {
            if (contextName == null) throw new ArgumentNullException("contextName");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (value == null) throw new ArgumentNullException("value");
            ContextName = contextName;
            PropertyName = propertyName;
            Value = value;
        }

        /// <summary>
        ///     Collection that the field belongs in
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        ///     Property name like "BrowserVersion"
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///     Value
        /// </summary>
        public object Value { get; set; }
    }
}