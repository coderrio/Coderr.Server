using System;
using Griffin.Container;

namespace OneTrueError.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Checks if WCF is loaded.
    /// </summary>
    [Component]
    internal class WcfIdentifier : ITagIdentifier
    {
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.AddIfFound("System.ServiceModel", "wcf");
        }
    }
}