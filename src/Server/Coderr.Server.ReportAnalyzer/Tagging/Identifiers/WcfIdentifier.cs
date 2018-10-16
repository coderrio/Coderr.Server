using System;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Tagging.Identifiers
{
    /// <summary>
    ///     Checks if WCF is loaded.
    /// </summary>
    [ContainerService]
    internal class WcfIdentifier : ITagIdentifier
    {
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.AddIfFound("System.ServiceModel", "wcf");
        }
    }
}