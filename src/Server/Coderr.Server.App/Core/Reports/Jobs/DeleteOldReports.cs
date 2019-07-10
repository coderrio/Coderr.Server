using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.App.Core.Reports.Config;
using Griffin.ApplicationServices;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.App.Core.Reports.Jobs
{
    /// <summary>
    ///     Will delete all reports which is older than the configured (<see cref="ReportConfig.RetentionDays" />) retention
    ///     period.
    /// </summary>
    [ContainerService(RegisterAsSelf = true)]
    public class DeleteOldReports : IBackgroundJobAsync
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeleteOldReports));
        private readonly IDbConnection _connection;
        private readonly IConfiguration<ReportConfig> _reportConfig;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteOldReports" />.
        /// </summary>
        /// <param name="connection">Used for SQL queries</param>
        /// <param name="reportConfig"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DeleteOldReports(IDbConnection connection, IConfiguration<ReportConfig> reportConfig)
        {
            _connection = connection;
            _reportConfig = reportConfig;
        }

        /// <summary>
        ///     Number of days to keep old reports.
        /// </summary>
        public int RetentionDays => _reportConfig.Value.RetentionDays;

        /// <inheritdoc />
        public async Task ExecuteAsync()
        {
            using (var cmd = _connection.CreateDbCommand())
            {
                var sql = @"CREATE TABLE #OldReports 
                            ( 
                                ReportId int NOT NULL PRIMARY KEY
                            )

                            INSERT #OldReports (ReportId)
                                SELECT TOP(1000) Id
                                FROM ErrorReports WITH (READUNCOMMITTED)
                                WHERE CreatedAtUtc < @date

                            declare @counter int = 0;
 
                            IF @@ROWCOUNT <> 0 
                            BEGIN 
                                DECLARE OldReportsCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY
                                FOR SELECT ReportId FROM #OldReports 
                                set @counter = 1
     
                                DECLARE @ReportId int

                                OPEN OldReportsCursor

                                FETCH NEXT FROM OldReportsCursor INTO @ReportId

                                WHILE @@FETCH_STATUS = 0 
                                BEGIN
                                    set @counter = @counter + 1
                                    DELETE FROM ErrorReports WHERE Id = @ReportId
                                    FETCH NEXT FROM OldReportsCursor INTO @ReportId
                                END

                                CLOSE OldReportsCursor

                                DEALLOCATE OldReportsCursor

                            END 

                            DROP TABLE #OldReports
                            select @counter;
                            ";
                cmd.CommandText = sql;
                cmd.AddParameter("date", DateTime.UtcNow.AddDays(-RetentionDays));
                cmd.CommandTimeout = 90;
                try
                {
                    var rows = await cmd.ExecuteNonQueryAsync();
                    if (rows > 0)
                    {
                        _logger.Debug("Deleted the oldest " + rows + " reports.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed on connection " + cmd.Connection.ConnectionString, ex);
                    throw;
                }
            }
        }

    }
}