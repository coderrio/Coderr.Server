using System;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Looks for the EntityFramework.SqlServer assembly in the stack trace.
    /// </summary>
    [Component]
    public class EntityFrameworkIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var orderNumber = context.AddIfFound("EntityFramework", "entity-framework");
            if (orderNumber != -1)
            {
                var propertyValue = context.GetPropertyValue("Assemblies", "entity-framework");
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    context.AddTag("entity-framework-" + propertyValue.Substring(0, 1), orderNumber);
                }
            }
            var property2 = context.GetPropertyValue("Assemblies", "EntityFramework.SqlServer");
            if (property2 != null)
            {
                context.AddTag("sqlserver", 99);
            }
        }
    }
}