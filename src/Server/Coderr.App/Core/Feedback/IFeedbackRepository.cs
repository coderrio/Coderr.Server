using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace codeRR.Server.App.Core.Feedback
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
        Task<FeedbackEntity> FindPendingAsync(string reportId);

        /// <summary>
        ///     Get all email addresses associated with an incident
        /// </summary>
        /// <param name="incidentId">incident</param>
        /// <returns>emails (or an emty list)</returns>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IReadOnlyList<string>> GetEmailAddressesAsync(int incidentId);

        /// <summary>
        ///     Update feedback
        /// </summary>
        /// <param name="feedback">entity</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">feedback</exception>
        Task UpdateAsync(FeedbackEntity feedback);
    }
}