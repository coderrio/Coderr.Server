using System.Data;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Reports;
using Griffin.ApplicationServices;
using Griffin.Data;
using log4net;

namespace Coderr.Server.App.Core.Reports.Jobs
{
    /// <summary>
    ///     Delete oldest reports for incidents with report count cap.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You can configure the amount of reports per incident in the admin area.
    ///     </para>
    /// </remarks>
    [ContainerService(RegisterAsSelf = true)]
    public class DeleteReportsBelowReportLimit : IBackgroundJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeleteReportsBelowReportLimit));
        private readonly IDbConnection _connection;
        private readonly IConfiguration<ReportConfig> _reportConfig;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteReportsBelowReportLimit" />.
        /// </summary>
        /// <param name="connection">Used for SQL queries</param>
        public DeleteReportsBelowReportLimit(IDbConnection connection, IConfiguration<ReportConfig> reportConfig)
        {
            _connection = connection;
            _reportConfig = reportConfig;
        }

        /// <summary>
        ///     Number of reports which can be stored per incident.
        /// </summary>
        public int MaxReportsPerIncident
        {
            get
            {
                return _reportConfig?.Value?.MaxReportsPerIncident ?? 100;
            }
        }

        /// <inheritdoc />
        public void Execute()
        {
            using (var cmd = _connection.CreateCommand())
            {
                var sql = $@"CREATE TABLE #Incidents (Id int NOT NULL PRIMARY KEY, NumberOfItems int)
                            INSERT #Incidents (Id, NumberOfItems)
                            SELECT TOP(100) IncidentId, Count(Id) - @max
                            FROM ErrorReports WITH (READUNCOMMITTED)
                            GROUP BY IncidentId
                            HAVING Count(Id) > @max
                            ORDER BY count(Id) DESC

                            CREATE TABLE #ReportsToDelete (Id int not null primary key)
                            declare @counter int = 0;

                            DECLARE IncidentCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY
                            FOR SELECT Id, NumberOfItems FROM #Incidents
                            DECLARE @IncidentId int
                            DECLARE @NumberOfItems int
                            OPEN IncidentCursor
                            FETCH NEXT FROM IncidentCursor INTO @IncidentId, @NumberOfItems
                            WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                INSERT INTO #ReportsToDelete (Id)
                                SELECT TOP(@NumberOfItems) Id
                                    FROM ErrorReports WITH (READUNCOMMITTED)
                                    WHERE IncidentId = @IncidentId
                                    ORDER BY Id asc
                                FETCH NEXT FROM IncidentCursor INTO @IncidentId, @NumberOfItems
                            END
                            CLOSE IncidentCursor
                            DEALLOCATE IncidentCursor
                            DROP TABLE #Incidents

                            DECLARE ItemsToDeleteCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY
                            FOR SELECT Id FROM #ReportsToDelete

                            DECLARE @IdToDelete int
                            OPEN ItemsToDeleteCursor
                            FETCH NEXT FROM ItemsToDeleteCursor INTO @IdToDelete

                            WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                set @counter = @counter + 1
                                DELETE FROM ErrorReports WHERE Id = @IdToDelete
                                FETCH NEXT FROM ItemsToDeleteCursor INTO @IdToDelete
                            END

                            CLOSE ItemsToDeleteCursor
                            DEALLOCATE ItemsToDeleteCursor
                            DROP TABLE #ReportsToDelete
                            select @counter;";

                cmd.CommandText = sql;
                cmd.CommandTimeout = 90;
                cmd.AddParameter("max", MaxReportsPerIncident);
                var rows = (int)cmd.ExecuteScalar();
                if (rows > 0)
                {
                    _logger.Debug("Deleted the oldest " + rows + " reports.");
                }

            }
        }
    }
}