using System;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Incidents;

namespace Coderr.Server.ReportAnalyzer
{
    /// <summary>
    ///     Repository (think CQRS write side in this case. yay!)
    /// </summary>
    public interface IAnalyticsRepository
    {
        /// <summary>
        /// Save an environment
        /// </summary>
        /// <param name="incidentId">incident that the report is for</param>
        /// <param name="environmentName">Name as specified by the developer</param>
        void SaveEnvironmentName(int incidentId, string environmentName);

        /// <summary>
        ///     Create a new incident
        /// </summary>
        /// <param name="incidentAnalysis">incident to persist</param>
        /// <exception cref="ArgumentNullException">incentAnalysis</exception>
        void CreateIncident(IncidentBeingAnalyzed incidentAnalysis);

        /// <summary>
        ///     Create a new error report
        /// </summary>
        /// <param name="report">report to persist</param>
        /// <exception cref="ArgumentNullException">report</exception>
        void CreateReport(ErrorReportEntity report);

        /// <summary>
        ///     There is an incident for the given report error id.
        /// </summary>
        /// <param name="clientReportId">error id for a report, generated in the client library</param>
        /// <returns><c>true</c> if an incident exists; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">clientReportId</exception>
        bool ExistsByClientId(string clientReportId);

        /// <summary>
        ///     Find incident
        /// </summary>
        /// <param name="applicationId">application that the incident belongs to</param>
        /// <param name="reportHashCode">generated hash code</param>
        /// <param name="hashCodeIdentifier">
        ///     Line to use if multiple incidents (typically first line in the exception error
        ///     message) have the same hash code
        /// </param>
        /// <returns>incident if found; otherwise <c>null</c>.</returns>
        IncidentBeingAnalyzed FindIncidentForReport(int applicationId, string reportHashCode, string hashCodeIdentifier);

        /// <summary>
        ///     Get application name
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>name</returns>
        string GetAppName(int applicationId);

        /// <summary>
        ///     Update incident
        /// </summary>
        /// <param name="incidentAnalysis">incident to persist</param>
        /// <exception cref="ArgumentNullException">incidentAnalysis</exception>
        void UpdateIncident(IncidentBeingAnalyzed incidentAnalysis);

        /// <summary>
        /// Number of reports received this month
        /// </summary>
        /// <returns></returns>
        int GetMonthReportCount();

        void AddMissedReport(DateTime date);
        Task StoreReportStats(ReportMapping mapping);
    }
}