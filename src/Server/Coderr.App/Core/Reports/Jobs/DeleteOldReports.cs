using System;
using System.Threading.Tasks;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;
using codeRR.App.Core.Reports.Config;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Core.Reports.Jobs
{
    /// <summary>
    ///     Will delete all reports which is older than the configured (<see cref="ReportConfig.RetentionDays" />) retention
    ///     period.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class DeleteOldReports : IBackgroundJobAsync
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeleteOldReports));
        private readonly IAdoNetUnitOfWork _unitOfWork;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteOldReports" />.
        /// </summary>
        /// <param name="unitOfWork">Used for SQL queries</param>
        public DeleteOldReports(IAdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Number of reports which can be stored per incident.
        /// </summary>
        public int MaxReportsPerIncident
        {
            get { return ConfigurationStore.Instance.Load<ReportConfig>().MaxReportsPerIncident; }
        }

        /// <summary>
        ///     Number of days to keep old reports.
        /// </summary>
        public int RetentionDays
        {
            get
            {
                var config = ConfigurationStore.Instance.Load<ReportConfig>();
                return config != null ? config.RetentionDays : 90;
            }
        }

        /// <inheritdoc />
        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                var sql = @"DELETE TOP(1000) FROM ErrorReports WHERE CreatedAtUtc < @date";
                cmd.CommandText = sql;
                cmd.AddParameter("date", DateTime.UtcNow.AddDays(-RetentionDays));
                cmd.CommandTimeout = 90;
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    _logger.Debug("Deleted the oldest " + rows + " reports.");
                }
            }
        }
    }
}