using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OneTrueError.App.Modules.Similarities.Domain.Adapters.Runner
{
    /// <summary>
    /// Loads similarity adapters by using reflection.
    /// </summary>
    public class AdapterRepository : IAdapterRepository
    {
        private static readonly List<Type> Adapters;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "How would I do that?")]
        static AdapterRepository()
        {
            Adapters = (from type in Assembly.GetExecutingAssembly().GetTypes()
                where type.IsClass
                      && !type.IsAbstract
                      && typeof (IValueAdapter).IsAssignableFrom(type)
                select type).ToList();
        }

        /// <summary>
        /// Get a list of all value adapters.
        /// </summary>
        /// <returns>All identified adapters (single instances)</returns>
        public IReadOnlyList<IValueAdapter> GetAdapters()
        {
            return Adapters.Select(Activator.CreateInstance).Cast<IValueAdapter>().ToList();
        }
    }
}