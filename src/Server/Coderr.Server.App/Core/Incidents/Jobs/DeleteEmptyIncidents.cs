using System;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Reports.Config;
using Coderr.Server.PluginApi.Config;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace codeRR.Server.App.Core.Incidents.Jobs
{
    /// <summary>
    ///     Delete incidents where all reports have been deleted (due to retention days).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         There are other jobs where old reports are removed. This job is to make sure that old incidents are being
    ///         deleted
    ///         when there are no reports for them. Do note that ignored incidents will not be deleted.
    ///     </para>
    /// </remarks>
    [Component(RegisterAsSelf = true)]
    internal class DeleteEmptyIncidents : IBackgroundJobAsync
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeleteEmptyIncidents));
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IConfiguration<ReportConfig> _reportConfiguration;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteEmptyIncidents" />.
        /// </summary>
        /// <param name="unitOfWork">Used for SQL queries</param>
        public DeleteEmptyIncidents(IAdoNetUnitOfWork unitOfWork, IConfiguration<ReportConfig> reportConfiguration)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            this._reportConfiguration = reportConfiguration;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync()
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    $@"DELETE TOP(1000) Incidents 
                       WHERE LastReportAtUtc < @retentionDays";

                // Wait until no reports have been received for the specified report save time
                // and then make sure during another period that no new reports have been received.
                var incidentRetention = _reportConfiguration.Value.RetentionDays * 2;

                cmd.AddParameter("retentionDays", DateTime.Today.AddDays(-incidentRetention));
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    _logger.Debug("Deleted " + rows + " empty incidents.");
                }
            }
        }
    }
}