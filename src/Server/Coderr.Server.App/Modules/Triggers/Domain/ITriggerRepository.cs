using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Repository to load information for the Trigger root aggregate.
    /// </summary>
    public interface ITriggerRepository
    {
        /// <summary>
        ///     Create a new trigger
        /// </summary>
        /// <param name="trigger">trigger</param>
        /// <returns>task</returns>
        Task CreateAsync(Trigger trigger);

        /// <summary>
        ///     Create collection metadata
        /// </summary>
        /// <param name="collection">metadata</param>
        /// <returns>task</returns>
        Task CreateAsync(CollectionMetadata collection);

        /// <summary>
        ///     Delete a trigger
        /// </summary>
        /// <param name="id">trigger PK</param>
        /// <returns>task</returns>
        Task DeleteAsync(int id);

        /// <summary>
        ///     Get a trigger
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns>trigger</returns>
        /// <exception cref="EntityNotFoundException">Trigger was not found.</exception>
        Task<Trigger> GetAsync(int id);

        /// <summary>
        ///     Get collection metadata
        /// </summary>
        /// <param name="applicationId">application to load it for.</param>
        /// <returns>Metadata (or an empty collection)</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IList<CollectionMetadata>> GetCollectionsAsync(int applicationId);

        /// <summary>
        ///     Get all triggers for the given application
        /// </summary>
        /// <param name="applicationId">app PK</param>
        /// <returns></returns>
        IEnumerable<Trigger> GetForApplication(int applicationId);

        /// <summary>
        ///     Update trigger
        /// </summary>
        /// <param name="entity">trigger</param>
        /// <returns>task</returns>
        Task UpdateAsync(Trigger entity);

        /// <summary>
        ///     Update metadata
        /// </summary>
        /// <param name="collection">collection</param>
        /// <returns>task</returns>
        Task UpdateAsync(CollectionMetadata collection);
    }
}