using System;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Item for <see cref="FindIncidentsResult" />.
    /// </summary>
    public class FindIncidentsResultItem
    {
        /// <summary>
        ///     Creates new instance of <see cref="FindIncidentsResultItem" />.
        /// </summary>
        /// <param name="id">incident id</param>
        /// <param name="name">incident name</param>
        public FindIncidentsResultItem(int id, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected FindIncidentsResultItem()
        {
        }

        /// <summary>
        ///     Id of the application that this incident belongs to
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application that this incident belongs to
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     When the first report was received.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int Id { get; private set; }


        /// <summary>
        ///     Incident have been automatically opened again after being closed by a user.
        /// </summary>
        public bool IsReOpened { get; set; }

        /// <summary>
        ///     When someone updated this incident (assigned/closed etc).
        /// </summary>
        public DateTime LastUpdateAtUtc { get; set; }

        /// <summary>
        ///     Incident name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Total number of received reports (increased even if the number of stored reports are at the limit)
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        /// When we recieved the last report.
        /// </summary>
        public DateTime LastReportReceivedAtUtc { get; set; }
    }
}