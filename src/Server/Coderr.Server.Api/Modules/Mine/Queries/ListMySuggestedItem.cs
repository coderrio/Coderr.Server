using System;

namespace Coderr.Server.Api.Modules.Mine.Queries
{
    /// <summary>
    ///     Item for <see cref="ListMyIncidentsResult" />.
    /// </summary>
    public class ListMySuggestedItem
    {
        /// <summary>
        ///     Creates new instance of <see cref="ListMySuggestedItem" />.
        /// </summary>
        /// <param name="id">incident id</param>
        /// <param name="name">incident name</param>
        public ListMySuggestedItem(int id, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ListMySuggestedItem()
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
        ///     When the first report was received.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        public string ExceptionTypeName { get; set; }

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
        ///     Number of points for this item. the more the merrier.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        ///     Total number of received reports (increased even if the number of stored reports are at the limit)
        /// </summary>
        public int ReportCount { get; set; }

        public string StackTrace { get; set; }

        /// <summary>
        /// Why this item was suggested.
        /// </summary>
        public string Motivation { get; set; }
    }
}