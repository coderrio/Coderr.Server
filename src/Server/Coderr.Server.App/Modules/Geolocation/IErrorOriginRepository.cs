using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace codeRR.Server.App.Modules.Geolocation
{
    /// <summary>
    ///     Stores error origins
    /// </summary>
    /// <remarks>
    ///     TODO: Uses the IncidentTabl
    /// </remarks>
    public interface IErrorOriginRepository
    {
        /// <summary>
        ///     Create a new entry
        /// </summary>
        /// <param name="entity">origin</param>
        /// <param name="applicationId">Application that we received a report for</param>
        /// <param name="incidentId">incident that the report belongs to</param>
        /// <param name="reportId">report received that we got a location for</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">origin</exception>
        Task CreateAsync(ErrorOrigin entity, int applicationId, int incidentId, int reportId);

        /// <summary>
        ///     Find all origins for a specific incident
        /// </summary>
        /// <param name="incidentId">incident</param>
        /// <returns>All locations.</returns>
        Task<IList<ErrorOrginListItem>> FindForIncidentAsync(int incidentId);
    }
}