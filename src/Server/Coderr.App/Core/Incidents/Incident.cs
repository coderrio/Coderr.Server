﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.App.Core.Incidents
{
    /// <summary>
    ///     Keeps track of all occurrences of a single incident (i.e. error reports which generates the same hash code)
    /// </summary>
    public class Incident
    {
        private string _description;

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Incident()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Incident" />.
        /// </summary>
        /// <param name="applicationId">application that the incident was created for</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public Incident(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");

            ApplicationId = applicationId;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Application that this incident belongs to
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     When the incident was created in the client library.
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     Incident description.Typically first line of the exception message.
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return "Ooops Error!";

                return _description;
            }
            set { _description = value; }
        }

        /// <summary>
        ///     PK
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Do not accept any more reports for this exception
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Means that no notifications or reports should be saved on this incident
        ///     </para>
        /// </remarks>
        public bool IgnoreReports { get; private set; }

        /// <summary>
        ///     <see cref="IgnoreReports" /> was set to true at this time.
        /// </summary>
        public DateTime IgnoringReportsSinceUtc { get; private set; }

        /// <summary>
        ///     Person that wanted us to ignore reports.
        /// </summary>
        public string IgnoringRequestedBy { get; private set; }

        /// <summary>
        ///     Incident was marked as completed, but we've received another report for this incident
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Do not apply to ignored incidents.
        ///     </para>
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re")]
        public bool IsReopened { get; private set; }

        /// <summary>
        ///     If the solution can be shared with others.
        /// </summary>
        public bool IsSolutionShared { get; private set; }

        /// <summary>
        ///     If this incident has been fixed.
        /// </summary>
        public bool IsSolved { get; private set; }

        /// <summary>
        ///     <see cref="IsReopened" /> has been set to true, this tells when the incident was closed the last time.
        /// </summary>
        public DateTime LastSolutionAtUtc { get; private set; }

        /// <summary>
        ///     When it was reopened.
        /// </summary>
        /// <seealso cref="IsReopened" />
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re")]
        public DateTime ReopenedAtUtc { get; private set; }

        /// <summary>
        ///     Number of reports that have been received so far (counts up even if the max number of reports have been received
        ///     for this incident).
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        ///     Gets what was done to fix this error.
        /// </summary>
        public IncidentSolution Solution { get; private set; }

        /// <summary>
        ///     When the <see cref="Solution" /> was written-
        /// </summary>
        public DateTime SolvedAtUtc { get; private set; }

        /// <summary>
        ///     Stack trace from exception.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     When the incident was updated through the UI or when a new report was received (whatever change was made last
        ///     time).
        /// </summary>
        public DateTime UpdatedAtUtc { get; private set; }

        /// <summary>
        ///     Do not want to store reports or receive notifications for this incident.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <exception cref="System.ArgumentNullException">accountName</exception>
        public void IgnoreFutureReports(string accountName)
        {
            if (accountName == null) throw new ArgumentNullException("accountName");
            IgnoreReports = true;
            IgnoringReportsSinceUtc = DateTime.UtcNow;
            IgnoringRequestedBy = accountName;
        }

        /// <summary>
        ///     Dang! Got a new report after the incident being closed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re")]
        public void Reopen()
        {
            LastSolutionAtUtc = SolvedAtUtc;
            IsSolved = false;
            ReopenedAtUtc = DateTime.UtcNow;
            IsReopened = true;
            IgnoreReports = false;
            IgnoringReportsSinceUtc = DateTime.MinValue;
        }

        /// <summary>
        ///     Specifies that this solution can be shared with other projects.
        /// </summary>
        public void ShareSolution()
        {
            //TODO: Do something
            IsSolutionShared = true;
        }

        /// <summary>
        ///     Yay! One in the dev team figured out how the error can be solved.
        /// </summary>
        /// <param name="solvedBy">AccountId for whoever wrote the solution</param>
        /// <param name="solution">Actual solution</param>
        /// <exception cref="ArgumentNullException">solution</exception>
        /// <exception cref="ArgumentOutOfRangeException">solvedBy</exception>
        public void Solve(int solvedBy, string solution)
        {
            if (solution == null) throw new ArgumentNullException("solution");
            if (solvedBy <= 0) throw new ArgumentOutOfRangeException("solvedBy");

            Solution = new IncidentSolution(solvedBy, solution);
            UpdatedAtUtc = DateTime.UtcNow;
            SolvedAtUtc = DateTime.UtcNow;
            IsSolved = true;
        }
    }
}