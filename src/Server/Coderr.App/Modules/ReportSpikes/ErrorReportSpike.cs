using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace codeRR.Server.App.Modules.ReportSpikes
{
    /// <summary>
    ///     A spike is when we receive an unusual amount of reports during a short period of time for an application.
    /// </summary>
    public class ErrorReportSpike
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ErrorReportSpike" />
        /// </summary>
        /// <param name="applicationId">Application that the spike was detected for</param>
        /// <param name="count">Initial count when the spike was detetced</param>
        public ErrorReportSpike(int applicationId, int count)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            if (count <= 0) throw new ArgumentOutOfRangeException("count");

            ApplicationId = applicationId;
            Count = count;
            SpikeDate = DateTime.Today;
            NotifiedAccounts = new int[0];
        }

        /// <summary>
        ///     Application that the inspected incident belongs to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Number of error reports
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Accounts that we've sent notifications to for this spike.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "I like my arrays.")]
        public int[] NotifiedAccounts { get; private set; }

        /// <summary>
        ///     Date (and not DateTime) for the date when the spike was detected.
        /// </summary>
        /// <remarks>
        ///     <para>the purpose is to make sure that we do not send out several spike messages for the same spike</para>
        /// </remarks>
        public DateTime SpikeDate { get; private set; }

        /// <summary>
        ///     Add an account that has been notified
        /// </summary>
        /// <param name="accountId">account id</param>
        public void AddNotifiedAccount(int accountId)
        {
            if (HasAccount(accountId))
                throw new InvalidOperationException("Account have already been notified: " + accountId);
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            NotifiedAccounts = NotifiedAccounts.Concat(new[] {accountId}).ToArray();
        }

        /// <summary>
        ///     Check if the given account id have been previously notified for this spike.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool HasAccount(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            return NotifiedAccounts.Contains(accountId);
        }

        /// <summary>
        ///     Increase the number of reports that we have received since the spike was detected.
        /// </summary>
        public void IncreaseReportCount()
        {
            Count += 1;
        }
    }
}