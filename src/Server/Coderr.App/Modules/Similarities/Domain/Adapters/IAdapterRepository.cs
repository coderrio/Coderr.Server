using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Keeps track of all adapters which are used to normalize the incoming values to be able to find similarities.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IAdapterRepository
    {
        /// <summary>
        ///     Get a list of all value adapters.
        /// </summary>
        /// <returns>All identified adapters (single instances)</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IReadOnlyList<IValueAdapter> GetAdapters();
    }
}