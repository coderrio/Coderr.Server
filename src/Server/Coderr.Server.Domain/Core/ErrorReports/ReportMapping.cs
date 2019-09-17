using System;

namespace Coderr.Server.Domain.Core.ErrorReports
{
    /// <summary>
    ///     Maps a received report to an incident
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Since we do not store complete reports for everything any more. Allows us to still keep
    ///         track of client side generated error ids and which incident they belong to.
    ///     </para>
    /// </remarks>
    public class ReportMapping
    {
        public int Id { get; set; }

        /// <summary>
        /// Incident that the report belongs to.
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        /// Client report id
        /// </summary>
        public string ErrorId { get; set; }

        /// <summary>
        /// Only when fetching items
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// When the report was generated in the client
        /// </summary>
        public DateTime ReceivedAtUtc { get; set; }
    }
}