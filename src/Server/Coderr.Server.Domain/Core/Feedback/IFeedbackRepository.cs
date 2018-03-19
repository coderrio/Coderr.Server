using System;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Core.Feedback
{
    /// <summary>
    ///     Feedback
    /// </summary>
    public interface IFeedbackRepository
    {
        /// <summary>
        ///     Find pending feedback (i.e. have not got a matching report yet)
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns>entity if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">reportId</exception>
        Task<UserFeedback> FindPendingAsync(string reportId);

        /// <summary>
        ///     Update feedback
        /// </summary>
        /// <param name="feedback">entity</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">feedback</exception>
        Task UpdateAsync(UserFeedback feedback);
    }
}