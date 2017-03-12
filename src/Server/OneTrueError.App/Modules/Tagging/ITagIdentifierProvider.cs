using System;
using System.Collections.Generic;

namespace OneTrueError.App.Modules.Tagging
{
    public interface ITagIdentifierProvider
    {
        /// <summary>
        ///     Get all identifies for the given context.
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>all identified identifiers.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        IEnumerable<ITagIdentifier> GetIdentifiers(TagIdentifierContext context);
    }
}