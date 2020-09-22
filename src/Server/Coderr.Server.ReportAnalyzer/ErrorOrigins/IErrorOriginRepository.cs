using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.ErrorOrigins;

namespace Coderr.Server.ReportAnalyzer.ErrorOrigins
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

        Task<IList<ErrorOrigin>> GetPendingOrigins();

        Task Update(ErrorOrigin entity);
    }
}
