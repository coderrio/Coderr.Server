using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace codeRR.Server.App.Modules.Tagging
{
    /// <summary>
    ///     Used to provide tag identifiers (i.e. classes which can identify StackOverflow.com tags by using the report
    ///     information)
    /// </summary>
    [Obsolete]
    public class AssemblyScanningTagIdentifierProvider : ITagIdentifierProvider
    {
        private static readonly List<ITagIdentifier> Identifiers = new List<ITagIdentifier>();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "How on earth could I do that?")]
        static AssemblyScanningTagIdentifierProvider()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !typeof(ITagIdentifier).IsAssignableFrom(type))
                    continue;

                Identifiers.Add((ITagIdentifier) Activator.CreateInstance(type));
            }
        }

        /// <summary>
        ///     Get all identifies for the given context.
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>all identified identifiers.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IEnumerable<ITagIdentifier> GetIdentifiers(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            return Identifiers;
        }
    }
}