using System;

namespace OneTrueError.Api.Core.Incidents.Queries
{
    /// <summary>
    /// A day in our statistics
    /// </summary>
    public class ReportDay
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Number of items this day
        /// </summary>
        public int Count { get; set; }
    }
}