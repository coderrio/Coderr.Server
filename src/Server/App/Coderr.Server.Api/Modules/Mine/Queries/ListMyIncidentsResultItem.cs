using System;

namespace Coderr.Server.Api.Modules.Mine.Queries
{
    /// <summary>
    ///     Item for <see cref="ListMyIncidents" />.
    /// </summary>
    public class ListMyIncidentsResultItem
    {
        /// <summary>
        ///     Creates new instance of <see cref="ListMyIncidentsResultItem" />.
        /// </summary>
        /// <param name="id">incident id</param>
        /// <param name="name">incident name</param>
        public ListMyIncidentsResultItem(int id, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ListMyIncidentsResultItem()
        {
        }

        /// <summary>
        ///     Id of the application that this incident belongs to
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application that this incident belongs to
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     when this incident was assigned to me.
        /// </summary>
        public DateTime AssignedAtUtc { get; set; }

        /// <summary>
        ///     When the first report was received.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        public int DaysOld => (int)DateTime.UtcNow.Subtract(CreatedAtUtc).TotalDays;

        /// <summary>
        ///     Incident id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     When the last report was received (or when the last user action was made)
        /// </summary>
        public DateTime LastReportAtUtc { get; set; }

        /// <summary>
        ///     Incident name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Total number of received reports (increased even if the number of stored reports are at the limit)
        /// </summary>
        public int ReportCount { get; set; }
    }
}