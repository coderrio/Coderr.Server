using System;
using System.Collections.Generic;
using codeRR.Server.App.Core.Reports.Config;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace codeRR.Server.App.Core.Reports.Jobs
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
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteReportsBelowReportLimit" />.
        /// </summary>
        /// <param name="unitOfWork">Used for SQL queries</param>
        public DeleteReportsBelowReportLimit(IAdoNetUnitOfWork unitOfWork, ConfigurationStore configStore)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            _configStore = configStore;
        }

        /// <summary>
        ///     Number of reports which can be stored per incident.
        /// </summary>
        public int MaxReportsPerIncident
        {
            get
            {
                var config = _configStore.Load<ReportConfig>();
                if (config == null)
                    return 100;
                return config.MaxReportsPerIncident;
            }
        }

        /// <inheritdoc />
        public void Execute()
        {
            // find incidents with too many reports.
            var incidentsToTruncate = new List<Tuple<int, int>>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT TOP(5) IncidentId, count(Id)
                        FROM ErrorReports WITH (ReadPast)
                        GROUP BY IncidentId
                        HAVING Count(IncidentId) > @max
                        ORDER BY count(Id) DESC";
                cmd.AddParameter("max", MaxReportsPerIncident);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        incidentsToTruncate.Add(new Tuple<int, int>((int)reader[0], (int)reader[1]));
                    }
                }
            }

            foreach (var incidentIdAndCount in incidentsToTruncate)
            {
                //do not delete more then 500 at a time.
                var rowsToDelete = Math.Min(500, incidentIdAndCount.Item2 - MaxReportsPerIncident);
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    var sql = $@"With RowsToDelete AS
                                (
                                    SELECT TOP {rowsToDelete} Id
                                    FROM ErrorReports WITH (ReadPast)
                                    WHERE IncidentId = {incidentIdAndCount.Item1}
                                )
                                DELETE FROM RowsToDelete";
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 90;
                    var rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        _logger.Debug("Deleted the oldest " + rows + " reports for incident " + incidentIdAndCount);
                    }
                }
            }
        }
    }
}