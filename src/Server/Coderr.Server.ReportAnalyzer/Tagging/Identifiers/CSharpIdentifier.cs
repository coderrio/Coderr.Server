using System;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Tagging.Identifiers
{
    /// <summary>
    ///     Checks if C# is loaded and which version of it.
    /// </summary>
    [ContainerService]
    internal class CSharpIdentifier : ITagIdentifier
    {
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            var property = context.GetPropertyValue("Assembly", "Microsoft.CSharp");
            if (property == null)
                return;

            context.AddTag("c#", 99);
            var pos = property.IndexOf(".");
            if (pos != -1)
                context.AddTag("c#-" + property.Substring(pos + 1, 3), 99);
        }
    }
}