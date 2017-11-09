using System;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Keeps track of all occurrences of a single incident (i.e. error reports which generates the same hash code)
    /// </summary>
    public class GetIncidentResult
    {
        private string _description;


        /// <summary>
        ///     Application that the incident belongs to
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     When it was assigned to the person.
        /// </summary>
        public DateTime? AssignedAtUtc { get; set; }

        /// <summary>
        ///     User name of the person that this incident is assigned to.
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        ///     User assigned to the incident.
        /// </summary>
        public int? AssignedToId { get; set; }

        /// <summary>
        ///     Context collection names.
        /// </summary>
        public string[] ContextCollections { get; set; }

        /// <summary>
        ///     When the incident was created (when we received the first exception).
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     Daily statistics.
        /// </summary>
        public ReportDay[] DayStatistics { get; set; }

        /// <summary>
        ///     Error description (exception message)
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return "Ooops Error!";

                return _description;
            }
            set => _description = value;
        }

        /// <summary>
        ///     facts
        /// </summary>
        public QuickFact[] Facts { get; set; }

        /// <summary>
        ///     Full name of the exception message.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        ///     Used to identify this incident when the hash code is the same as for other incidents.
        /// </summary>
        /// <returns></returns>
        public string HashCodeIdentifier { get; private set; }

        /// <summary>
        ///     primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Stores the state temporary to be able to assigned the bool fields
        /// </summary>
        [IgnoreField]
        public int IncidentState { get; set; }

        /// <summary>
        ///     Ignore future reports for this incident (i.e. no notifications, do not store new reports etc).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Report counter will still be updated.
        ///     </para>
        /// </remarks>
        public bool IsIgnored => IncidentState == 2;

        /// <summary>
        ///     If the incident was closed and then received error reports again.
        /// </summary>
        public bool IsReOpened { get; set; }

        /// <summary>
        ///     Share solution with the codeRR community.
        /// </summary>
        public bool IsSolutionShared { get; set; }

        /// <summary>
        ///     Incident has been marked as solved (i.e. closed)
        /// </summary>
        public bool IsSolved => IncidentState == 3;

        /// <summary>
        ///     When we received the last report for this incident.
        /// </summary>
        public DateTime LastReportReceivedAtUtc { get; set; }

        /// <summary>
        ///     Solution written last time (if <see cref="IsReOpened" /> is <c>true</c>).
        /// </summary>
        public DateTime PreviousSolutionAtUtc { get; set; }

        /// <summary>
        ///     Date if <see cref="IsReOpened" /> is <c>true</c>.
        /// </summary>
        public DateTime ReOpenedAtUtc { get; set; }


        /// <summary>
        ///     Number of reports received to date.
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        ///     Generated hash code
        /// </summary>
        public string ReportHashCode { get; private set; }

        /// <summary>
        ///     How the incident was solved (the last time)
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        ///     When the incident was closed/solved.
        /// </summary>
        public DateTime SolvedAtUtc { get; set; }

        /// <summary>
        ///     Stack trace.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     Identified StackOverflow tags.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        ///     When the incident was updated (either a new report or changes to the actual incident)
        /// </summary>
        public DateTime UpdatedAtUtc { get; private set; }

        public SuggestedIncidentSolution[] SuggestedSolutions { get; set; }
        public HighlightedContextData[] HighlightedContextData { get; set; }
    }
}