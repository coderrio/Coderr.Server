using System.Collections.Generic;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace OneTrueError.ReportAnalyzer.CustomerJobs
{
    /// <summary>
    /// Delete oldest reports for incidents with report count cap.
    /// </summary>
    [Component]
    class DeleteReportsBelowReportLimit
    {
        private IAdoNetUnitOfWork _unitOfWork;
        private ILog _logger = LogManager.GetLogger(typeof(DeleteReportsBelowReportLimit));

        public DeleteReportsBelowReportLimit(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            // find incidents with too many reports.
            var incidentsToTruncate = new List<int>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT TOP 10 Id FROM Incidents WHERE ReportCount > 5000 ORDER BY ReportCount DESC";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        incidentsToTruncate.Add((int)reader[0]);
                    }
                }
            }

            foreach (var incidentId in incidentsToTruncate)
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.CommandText =
                        @"DELETE FROM ErrorReports WHERE IncidentId = @id AND Id <= (SELECT Id 
FROM ErrorReports
WHERE IncidentId = @id
ORDER BY Id DESC
OFFSET 5000 ROWS
FETCH NEXT 1 ROW ONLY)";
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