using System;

namespace Coderr.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationInfo" />.
    /// </summary>
    public class GetApplicationInfoResult
    {
        /// <summary>
        ///     Application key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        ///     Type of application
        /// </summary>
        public TypeOfApplication ApplicationType { get; set; }

        /// <summary>
        ///     Application id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name of the application.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Shared secret, used together with <see cref="AppKey" /> to make sure that the reports come from the correct source.
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        ///     Total number of incidents for this application.
        /// </summary>
        public int TotalIncidentCount { get; set; }

        /// <summary>
        /// Number of full time developers working on this application (1.5 = one full time and one half time)
        /// </summary>
        /// <remarks>
        ///<para>
        ///null = not specified
        /// </para>
        /// </remarks>
        public decimal? NumberOfDevelopers { get; set; }

        /// <summary>
        /// Versions that we have received error reports for.
        /// </summary>
        public string[] Versions { get; set; }

        /// <summary>
        /// Got information to be able to compare how the team is performing with other teams.
        /// </summary>
        public bool ShowStatsQuestion { get; set; }

        /// <summary>
        /// Number of days to keep new incidents before deleting them.
        /// </summary>
        /// <remarks>
        ///<para>
        /// We've seen that errors that aren't within 60 days aren't fixed at all. It's therefore better to delete them than to keep them in the system (and adding noise to the error list).
        /// </para>
        /// </remarks>
        public int RetentionDays { get; set; }

        /// <summary>
        /// When we received the last incident.
        /// </summary>
        public DateTime? LastIncidentAtUtc { get; set; }
    }
}