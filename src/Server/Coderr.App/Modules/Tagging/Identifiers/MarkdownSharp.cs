using System;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Identifies the MarkdownSharp library
    /// </summary>
    public class MarkdownSharpIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.AddIfFound("MarkdownSharp", "MarkdownSharp");
        }
    }
}