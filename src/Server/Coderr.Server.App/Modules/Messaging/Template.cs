using System.Collections.Generic;
using System.IO;

namespace codeRR.Server.App.Modules.Messaging
{
    /// <summary>
    ///     Message template (contains markdown, view model instructions etc).
    /// </summary>
    public class Template
    {
        /// <summary>
        ///     Template content
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        ///     Resources embedded in the template.
        /// </summary>
        public Dictionary<string, Stream> Resources { get; set; }
    }
}