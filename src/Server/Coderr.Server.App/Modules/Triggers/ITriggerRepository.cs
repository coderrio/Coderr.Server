using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data;

namespace Coderr.Server.App.Modules.Triggers
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
    }
}
