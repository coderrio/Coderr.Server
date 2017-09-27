using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Core.Reports.Invalid;
using Griffin.Data;

namespace codeRR.Server.App.Core.Reports
{
    /// <summary>
    ///     Repository for received error reports.
    /// </summary>
    public interface IReportsRepository
    {
        /// <summary>
        ///     Create a new invalid report
        /// </summary>
        /// <param name="invalidReport">report</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">invalidReport</exception>
        Task CreateAsync(InvalidErrorReport invalidReport);

        /// <summary>
        ///     Finds the by error identifier asynchronous.
        /// </summary>
        /// <param name="errorId">Customer generated id (from the client library).</param>
        /// <returns>report if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">errorId</exception>
        Task<ReportDTO> FindByErrorIdAsync(string errorId);


        /// <summary>
        ///     Get report
        /// </summary>
        /// <param name="id">report id</param>
        /// <returns>report</returns>
        /// <exception cref="ArgumentOutOfRangeException">id</exception>
        /// <exception cref="EntityNotFoundException">no report with that id</exception>
        Task<ReportDTO> GetAsync(int id);

        /// <summary>
        ///     Get a list of reports
        /// </summary>
        /// <param name="incidentId">incidentId</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">items per page</param>
        /// <returns>Paged reports</returns>
        /// <exception cref="ArgumentOutOfRangeException">incidentId &lt;= 0</exception>
        Task<PagedReports> GetForIncidentAsync(int incidentId, int pageNumber, int pageSize);
    }
}