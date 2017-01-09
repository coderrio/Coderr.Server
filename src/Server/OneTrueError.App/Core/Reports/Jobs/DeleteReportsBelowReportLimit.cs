using System;
using System.Collections.Generic;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;
using OneTrueError.App.Core.Reports.Config;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Core.Reports.Jobs
{
    /// <summary>
    ///     Delete oldest reports for incidents with report count cap.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You can configure the amount of reports per incident in the admin area.
    ///     </para>
    /// </remarks>
    [Component(RegisterAsSelf = true)]
    public class DeleteReportsBelowReportLimit : IBackgroundJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeleteReportsBelowReportLimit));
        private readonly IAdoNetUnitOfWork _unitOfWork;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteReportsBelowReportLimit" />.
        /// </summary>
        /// <param name="unitOfWork">Used for SQL queries</param>
        public DeleteReportsBelowReportLimit(IAdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Number of reports which can be stored per incident.
        /// </summary>
        public int MaxReportsPerIncident
        {
            get
            {
                var config = ConfigurationStore.Instance.Load<ReportConfig>();
                if (config == null)
                    return 5000;
                return config.MaxReportsPerIncident;
            }
        }

        /// <inheritdoc />
        public void Execute()
        {
            // find incidents with too many reports.
            var incidentsToTruncate = new List<int>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT TOP 10 Id FROM Incidents WHERE ReportCount > @max ORDER BY ReportCount DESC";
                cmd.AddParameter("max", MaxReportsPerIncident);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        incidentsToTruncate.Add((int) reader[0]);
                    }
                }
            }

            foreach (var incidentId in incidentsToTruncate)
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    var sql = @"DELETE FROM ErrorReports WHERE IncidentId = @id AND Id <= (SELECT Id 
FROM ErrorReports
WHERE IncidentId = @id
ORDER BY Id DESC
OFFSET {0} ROWS
FETCH NEXT 1 ROW ONLY)";
                    cmd.CommandText = string.Format(sql, MaxReportsPerIncident);
                    cmd.AddParameter("id", incidentId);
                    cmd.CommandTimeout = 90;
                    var rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        _logger.Debug("Deleted the oldest " + rows + " reports for incident " + incidentId);
                    }
                }

                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.CommandText =
                        @"UPDATE Incidents SET ReportCount = (SELECT count(Id) FROM ErrorReports WHERE IncidentId = @id) WHERE Id = @id";
                    cmd.AddParameter("id", incidentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}