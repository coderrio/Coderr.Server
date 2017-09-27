using System;

namespace codeRR.Server.App.Modules.Versions
{
    /// <summary>
    ///     Tracks number of incidents/reports for a specific year/month
    /// </summary>
    public class ApplicationVersionMonth
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationVersionMonth" />.
        /// </summary>
        /// <param name="versionId">Id from <see cref="ApplicationVersion" /></param>
        /// <param name="yearMonth">Which year/Month this entry tracks</param>
        public ApplicationVersionMonth(int versionId, DateTime yearMonth)
        {
            if (versionId <= 0) throw new ArgumentOutOfRangeException("versionId");
            if (yearMonth.Day != 1)
                throw new ArgumentException("Day must be set to 1", "yearMonth");

            VersionId = versionId;
            YearMonth = yearMonth;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected ApplicationVersionMonth()
        {
            
        }

        /// <summary>
        ///     Id for this year/month entry
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Number of new incidents this month
        /// </summary>
        public int IncidentCount { get; private set; }

        /// <summary>
        ///     When we received a report
        /// </summary>
        public DateTime LastUpdateAtUtc { get; set; }

        /// <summary>
        ///     Number of reports this month
        /// </summary>
        public int ReportCount { get; private set; }

        /// <summary>
        ///     FK to <see cref="ApplicationVersion" />.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        ///     Which year/month this is for
        /// </summary>
        public DateTime YearMonth { get; private set; }

        /// <summary>
        ///     Increase incident count
        /// </summary>
        public void IncreaseIncidentCount()
        {
            IncidentCount++;
            LastUpdateAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Increase report count
        /// </summary>
        public void IncreaseReportCount()
        {
            ReportCount++;
            LastUpdateAtUtc = DateTime.UtcNow;
        }
    }
}