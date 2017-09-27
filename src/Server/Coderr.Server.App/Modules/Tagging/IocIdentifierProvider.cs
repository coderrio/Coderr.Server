using System;
using System.Collections.Generic;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging
{
    /// <summary>
    ///     Uses the Container to find all tag identifiers.
    /// </summary>
    [Component]
    public class IocIdentifierProvider : ITagIdentifierProvider
    {
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        ///     Creates a new instance of <see cref="IocIdentifierProvider" />.
        /// </summary>
        /// <param name="serviceLocator">locator to use, typically a scoped one.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public IocIdentifierProvider(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
            _serviceLocator = serviceLocator;
        }

        /// <inheritdoc />
        public IEnumerable<ITagIdentifier> GetIdentifiers(TagIdentifierContext context)
        {
            return _serviceLocator.ResolveAll<ITagIdentifier>();
        }
    }
}