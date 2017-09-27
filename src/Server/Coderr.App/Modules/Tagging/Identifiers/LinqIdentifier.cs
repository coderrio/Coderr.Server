using System;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Identifies if the exception had LINQ in the stack trace.
    /// </summary>
    [Component]
    public class LinqIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.AddIfFound("System.Linq.Enumerable", "linq");
        }
    }
}