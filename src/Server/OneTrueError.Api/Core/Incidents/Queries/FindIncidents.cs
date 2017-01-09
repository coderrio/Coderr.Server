using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Find incidents
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default query is only open incidents with 20 items per page.
    ///     </para>
    /// </remarks>
    public class FindIncidents : Query<FindIncidentResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="FindIncidents" />.
        /// </summary>
        public FindIncidents()
        {
            MaxDate = DateTime.MaxValue;
            Open = true;
            ItemsPerPage = 20;
        }

        /// <summary>
        ///     0 = find for all applications
        /// </summary>
        /// <value>
        ///     The application identifier.
        /// </value>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Include closed incidents
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        ///     Include ignored incidents
        /// </summary>
        public bool Ignored { get; set; }

        /// <summary>
        ///     Number of items per page.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        ///     End of period
        /// </summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        ///     Start of period
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        ///     Include open incidents
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        ///     Page to fetch (one based index)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///     Include reopened incidents
        /// </summary>
        public bool ReOpened { get; set; }

        /// <summary>
        ///     Sort order
        /// </summary>
        public bool SortAscending { get; set; }

        /// <summary>
        ///     Sort type
        /// </summary>
        public IncidentOrder SortType { get; set; }
    }
}