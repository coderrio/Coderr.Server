using Griffin.Container;
using Griffin.Data;
using log4net;

namespace OneTrueError.ReportAnalyzer.CustomerJobs
{
    /// <summary>
    /// Delete similarities which exists for reports that have been deleted.
    /// </summary>
    [Component]
    class DeleteAbandonedSimilarities
    {
        private IAdoNetUnitOfWork _unitOfWork;
        private ILog _logger = LogManager.GetLogger(typeof (DeleteAbandonedSimilarities));

        public DeleteAbandonedSimilarities(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"DELETE Incidents WHERE Id IN (select Incidents.Id
                        FROM Incidents 
                        LEFT JOIN ErrorReports ON (ErrorReports.IncidentId = Incidents.Id)
                        WHERE ErrorReports.Id IS NULL) AND IgnoreReports <> 1";
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    _logger.Debug("Deleted " + rows + " rows of abandoned similarities");
                }
            }
        }
    }
}