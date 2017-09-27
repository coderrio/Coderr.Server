using System;
using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Modules.ReportSpikes
{
    /// <summary>
    ///     Repostory for spikes.
    /// </summary>
    public interface IReportSpikeRepository
    {
        /// <summary>
        ///     Create spike
        /// </summary>
        /// <param name="spike">spike</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task CreateSpikeAsync(ErrorReportSpike spike);

        /// <summary>
        ///     Get average day count for application
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>count</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<int> GetAverageReportCountAsync(int applicationId);

        /// <summary>
        ///     Get a spike
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">Failed to find a spike for the specified application</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<ErrorReportSpike> GetSpikeAsync(int applicationId);

        /// <summary>
        ///     Get number of reports for today
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>count</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<int> GetTodaysCountAsync(int applicationId);

        /// <summary>
        ///     Update spike
        /// </summary>
        /// <param name="spike">spike</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">spike</exception>
        Task UpdateSpikeAsync(ErrorReportSpike spike);
    }
}