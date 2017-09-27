using System;
using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Core.Incidents
{
    /// <summary>
    ///     Incident repository
    /// </summary>
    public interface IIncidentRepository
    {
        /// <summary>
        ///     Get incident
        /// </summary>
        /// <param name="id">incident id</param>
        /// <returns>incident</returns>
        /// <exception cref="ArgumentOutOfRangeException">id</exception>
        /// <exception cref="EntityNotFoundException">No incident was found with the given key.</exception>
        Task<Incident> GetAsync(int id);

        /// <summary>
        ///     Count the number of incidents for the given application
        /// </summary>
        /// <param name="applicationId">application</param>
        /// <returns>total count of incidents</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<int> GetTotalCountForAppInfoAsync(int applicationId);

        /// <summary>
        ///     Update incident
        /// </summary>
        /// <param name="incident">incdient</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">incident</exception>
        Task UpdateAsync(Incident incident);
    }
}