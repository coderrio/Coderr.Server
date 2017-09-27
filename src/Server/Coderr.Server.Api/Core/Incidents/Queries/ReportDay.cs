using System;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     A day in our statistics
    /// </summary>
    public class ReportDay
    {
        /// <summary>
        ///     Number of items this day
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Date
        /// </summary>
        public DateTime Date { get; set; }
    }
}