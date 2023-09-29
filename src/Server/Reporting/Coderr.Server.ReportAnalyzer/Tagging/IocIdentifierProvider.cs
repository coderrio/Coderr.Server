using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.Tagging
{
    /// <summary>
    ///     Uses the Container to find all tag identifiers.
    /// </summary>
    [ContainerService]
    public class IocIdentifierProvider : ITagIdentifierProvider
    {
        private readonly ITagIdentifier[] _identifiers;

        /// <summary>
        ///     Creates a new instance of <see cref="IocIdentifierProvider" />.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IocIdentifierProvider(IEnumerable<ITagIdentifier> identifiers)
        {
            _identifiers = identifiers.ToArray();
        }

        /// <inheritdoc />
        public IEnumerable<ITagIdentifier> GetIdentifiers(TagIdentifierContext context)
        {
            return _identifiers;
        }
    }
}