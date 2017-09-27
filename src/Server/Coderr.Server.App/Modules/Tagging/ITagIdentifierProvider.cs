using System;
using System.Collections.Generic;

namespace codeRR.Server.App.Modules.Tagging
{
    /// <summary>
    ///     A tag identifier is run on every inbound report with the task to find a set of tags that tells what kind of
    ///     incident this is.
    /// </summary>
    public interface ITagIdentifierProvider
    {
        /// <summary>
        ///     Get all identifies for the given context.
        /// </summary>
        /// <param name="context">information about the report and the incident</param>
        /// <returns>all identified identifiers.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        IEnumerable<ITagIdentifier> GetIdentifiers(TagIdentifierContext context);
    }
}