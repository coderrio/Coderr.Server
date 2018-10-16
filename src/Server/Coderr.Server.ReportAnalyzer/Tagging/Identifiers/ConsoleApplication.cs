using System;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Tagging.Identifiers
{
    /// <summary>
    ///     Checks if the application was run as an console application.
    /// </summary>
    [ContainerService]
    public class ConsoleApplication : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (context.IsFound("System.Windows.Forms"))
                return;

            context.AddIfFound("Program.Main(String[] args)", "console-application");
        }
    }
}