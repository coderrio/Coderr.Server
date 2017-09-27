using System;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Checks if WinForms is loaded.
    /// </summary>
    [Component]
    public class WinFormsIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.AddIfFound("System.Windows.Forms", "winforms");
        }
    }
}