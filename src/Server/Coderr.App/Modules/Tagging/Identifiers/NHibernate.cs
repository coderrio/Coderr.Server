using System;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Identifies nhibernate, fluent-nhibernate and other nhibernate related assemblies.
    /// </summary>
    [Component]
    public class NHibernate : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.AddIfFound("FluentNHibernate", "fluent-nhibernate");
            context.AddIfFound("FluentNHibernate.Mapping", "fluent-nhibernate-mapping");
            context.AddIfFound("NHibernate.", "nhibernate");
            context.AddIfFound("NHibernate.Criterion", "nhibernate-criteria");
            context.AddIfFound("NHibernate.Linq", "linq-to-nhibernate");
            context.AddIfFound("NHibernate.Mapping.", "nhibernate-mapping");

            //linq-to-nhibernate
        }
    }
}