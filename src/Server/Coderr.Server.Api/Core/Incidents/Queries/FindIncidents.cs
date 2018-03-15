using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Find incidents
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default query is only open incidents with 20 items per page.
    ///     </para>
    /// </remarks>
    [Message]
    public class FindIncidents : Query<FindIncidentsResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="FindIncidents" />.
        /// </summary>
        public FindIncidents()
        {
            MaxDate = DateTime.MaxValue;
            ItemsPerPage = 20;
        }


        /// <summary>
        ///     Find incidents assigned to the specified user
        /// </summary>
        public int AssignedToId { get; set; }

        /// <summary>
        ///     Empty = find for all applications
        /// </summary>
        /// <value>
        ///     The application identifier.
        /// </value>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        ///     Find an incident with a specific collection
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Wildcard search is allowed, i.e. "http*"
        ///     </para>
        /// </remarks>
        public string ContextCollectionName { get; set; }

        /// <summary>
        ///     Find an incident with a specific property
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Wildcard search is allowed, i.e. "http*"
        ///     </para>
        /// </remarks>
        public string ContextCollectionPropertyName { get; set; }

        /// <summary>
        ///     Find an incident with a specific property value
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Wildcard search is allowed, i.e. "http*"
        ///     </para>
        /// </remarks>
        public string ContextCollectionPropertyValue { get; set; }

        /// <summary>
        ///     Will be searched in incident.message and report.stacktrace.
        /// </summary>
        public string FreeText { get; set; }

        /// <summary>
        ///     Been assigned to someone
        /// </summary>
        public bool IsAssigned { get; set; }

        /// <summary>
        ///     Include closed incidents
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        ///     Include ignored incidents
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        ///     Incidents that have not been assigned to someone (or closed/ignored).
        /// </summary>
        public bool IsNew { get; set; }

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

        /// <summary>
        ///     Incident should have all the specified tags
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        ///     Version (in the form of a version string i.e. "1.2.1")
        /// </summary>
        public string Version { get; set; }
    }
}