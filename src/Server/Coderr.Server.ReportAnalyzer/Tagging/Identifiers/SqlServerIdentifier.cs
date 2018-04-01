using System;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Abstractions;

namespace Coderr.Server.ReportAnalyzer.Tagging.Identifiers
{
    /// <summary>
    ///     Add the "sql-server" tag-
    /// </summary>
    [ContainerService]
    public class SqlServerIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.AddIfFound("System.Data.SqlClient", "sql-server");

            //TODO: Can we identify the version in some way?
        }
    }
}