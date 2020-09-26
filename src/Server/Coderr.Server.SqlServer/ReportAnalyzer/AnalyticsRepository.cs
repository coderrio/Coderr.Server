using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Security;
using Coderr.Server.ReportAnalyzer;
using Coderr.Server.ReportAnalyzer.Inbound.Handlers.Reports;
using Coderr.Server.ReportAnalyzer.Incidents;
using Coderr.Server.SqlServer.ReportAnalyzer.Jobs;
using Coderr.Server.SqlServer.Tools;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.ReportAnalyzer
{
    [ContainerService]
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AnalyticsRepository));
        private readonly ReportDtoConverter _reportDtoConverter = new ReportDtoConverter();
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private const int MaxCollectionSize = 2000000;

        public AnalyticsRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public bool ExistsByClientId(string clientReportId)
        {
            if (clientReportId == null) throw new ArgumentNullException("clientReportId");

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "select id from ErrorReports WHERE ErrorId = @Id";
                cmd.AddParameter("id", clientReportId);
                return cmd.ExecuteScalar() != null;
            }
        }

        public IncidentBeingAnalyzed FindIncidentForReport(int applicationId, string reportHashCode,
            string hashCodeIdentifier)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT Incidents.* 
                        FROM Incidents
                        WHERE ReportHashCode = @ReportHashCode
                           AND ApplicationId = @applicationId";


                cmd.AddParameter("ReportHashCode", reportHashCode);
                cmd.AddParameter("applicationId", applicationId);

                var incidents = cmd.ToList<IncidentBeingAnalyzed>();
                return incidents.FirstOrDefault(incident => incident.HashCodeIdentifier == hashCodeIdentifier);
            }
        }

        public void CreateIncident(IncidentBeingAnalyzed incident)
        {
            if (incident == null) throw new ArgumentNullException("incident");
            if (string.IsNullOrEmpty(incident.ReportHashCode))
                throw new InvalidOperationException("ReportHashCode is required to be able to detect duplicates");

            if (incident.LastReportAtUtc == DateTime.MinValue)
                incident.LastReportAtUtc = DateTime.UtcNow;
            if (incident.LastStoredReportUtc == DateTime.MinValue)
                incident.LastStoredReportUtc = DateTime.UtcNow;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO Incidents (ReportHashCode, ApplicationId, CreatedAtUtc, HashCodeIdentifier, StackTrace, ReportCount, UpdatedAtUtc, Description, FullName, IsReOpened, LastStoredReportUtc, LastReportAtUtc)" +
                    " VALUES (@ReportHashCode, @ApplicationId, @CreatedAtUtc, @HashCodeIdentifier, @StackTrace, @ReportCount, @UpdatedAtUtc, @Description, @FullName, 0, @LastStoredReportUtc, @LastReportAtUtc);select SCOPE_IDENTITY();";
                cmd.AddParameter("Id", incident.Id);
                cmd.AddParameter("ReportHashCode", incident.ReportHashCode);
                cmd.AddParameter("ApplicationId", incident.ApplicationId);
                cmd.AddParameter("CreatedAtUtc", incident.CreatedAtUtc);
                cmd.AddParameter("HashCodeIdentifier", incident.HashCodeIdentifier);
                cmd.AddParameter("ReportCount", incident.ReportCount);
                cmd.AddParameter("UpdatedAtUtc", incident.UpdatedAtUtc);
                cmd.AddParameter("Description", incident.Description);
                cmd.AddParameter("StackTrace", incident.StackTrace);
                cmd.AddParameter("FullName", incident.FullName);
                cmd.AddParameter("LastStoredReportUtc", incident.LastStoredReportUtc);
                cmd.AddParameter("LastReportAtUtc", incident.LastReportAtUtc);

                var id = (int) (decimal) cmd.ExecuteScalar();
                incident.GetType()
                    .GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(incident, id);
            }

            //_unitOfWork.Insert(incident);
        }

        public void SaveEnvironmentName(int incidentId, string environmentName)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"declare @environmentId int;
                                    select @environmentId = id 
                                    from Environments
                                    WHERE Name = @name;

                                    if @environmentId is null
                                    begin
                                        insert into Environments(Name) VALUES(@name);
                                        set @environmentId = scope_identity();
                                        insert into IncidentEnvironments (IncidentId, EnvironmentId) VALUES(@incidentId, @environmentId);
                                    end
                                    else
                                    begin
                                        INSERT INTO IncidentEnvironments (IncidentId, EnvironmentId)
                                        SELECT @incidentId, @environmentId
                                        WHERE NOT EXISTS (SELECT IncidentId, EnvironmentId FROM IncidentEnvironments
                                                         WHERE IncidentId=@incidentId AND EnvironmentId=@environmentId)
                                    end;";
                cmd.AddParameter("incidentId", incidentId);
                cmd.AddParameter("name", environmentName);
                var rows = cmd.ExecuteNonQuery();
                _logger.Debug($"saved environment {environmentName} for incident {incidentId}, affected: {rows}");
            }
        }

        public void UpdateIncident(IncidentBeingAnalyzed incident)
        {
            if (incident == null) throw new ArgumentNullException("incident");
            _unitOfWork.Update(incident);
        }

        public int GetMonthReportCount()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var to = DateTime.UtcNow;

                cmd.CommandText =
                    "SELECT count(*) FROM IncidentReports WHERE ReceivedAtUtc >= @from ANd ReceivedAtUtc <= @to";
                cmd.AddParameter("from", from);
                cmd.AddParameter("to", to);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void AddMissedReport(DateTime date)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"update IgnoredReports set NumberOfReports=NumberOfReports+1 WHERE date = @date;
                        IF @@ROWCOUNT=0 insert into IgnoredReports(NumberOfReports, Date) values(1, Convert(date, GetUtcDate()));";
                cmd.AddParameter("date", date.Date);
                cmd.ExecuteNonQuery();
            }
        }

        public async Task StoreReportStats(ReportMapping mapping)
        {
            await _unitOfWork.InsertAsync(mapping);
        }

        public void CreateReport(ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            if (string.IsNullOrEmpty(report.Title) && report.Exception != null)
            {
                report.Title = report.Exception.Message;
                if (report.Title == null)
                    report.Title = "[Exception message was not specified]";
                else if (report.Title.Length > 100)
                    report.Title = report.Title.Substring(0, 100);
            }

            var collections = new List<string>();
            foreach (var context in report.ContextCollections)
            {
                var data = EntitySerializer.Serialize(context);
                if (data.Length > MaxCollectionSize)
                {
                    var tooLargeCtx = new ErrorReportContextCollection(context.Name,
                        new Dictionary<string, string>()
                        {
                            {
                                "Error",
                                $"This collection was larger ({data.Length}bytes) than the threshold of {MaxCollectionSize}bytes"
                            }
                        });

                    data = EntitySerializer.Serialize(tooLargeCtx);
                }
                collections.Add(data);
            }

            _unitOfWork.Insert(report);

            var cols = string.Join(", ", collections);
            var inboound = new InboundCollection
            {
                JsonData = $"[{cols}]",
                ReportId = report.Id
            };
            _unitOfWork.Insert(inboound);
        }

        public string GetAppName(int applicationId)
        {
            if (applicationId < 1)
                throw new ArgumentOutOfRangeException("applicationId", applicationId, "AppId must be a PK");

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Applications WHERE Id = @id";
                cmd.AddParameter("id", applicationId);
                return (string) cmd.ExecuteScalar();
            }
        }
        
        public IReadOnlyList<ErrorReportEntity> GetReportsUsingSql()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT QueueReports.*
                                    FROM QueueReports
                                    ORDER BY QueueReports.Id";
                cmd.Limit(10);

                try
                {
                    var reports = new List<ErrorReportEntity>();
                    var idsToRemove = new List<int>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var json = "";
                            try
                            {
                                json = (string) reader["body"];
                                var report = _reportDtoConverter.LoadReportFromJson(json);
                                var newReport = _reportDtoConverter.ConvertReport(report, (int) reader["ApplicationId"]);
                                newReport.RemoteAddress = (string) reader["RemoteAddress"];

                                var claims = CoderrDtoSerializer.Deserialize<Claim[]>((string) reader["Claims"]);
                                newReport.User = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationTypes.Default));

                                reports.Add(newReport);
                                idsToRemove.Add(reader.GetInt32(0));
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Failed to deserialize " + json, ex);
                            }
                        }
                    }
                    if (idsToRemove.Any())
                        _unitOfWork.ExecuteNonQuery("DELETE FROM QueueReports WHERE Id IN (" +
                                                    string.Join(",", idsToRemove) + ")");
                    return reports;
                }
                catch (Exception ex)
                {
                    throw cmd.CreateDataException(ex);
                }
            }
        }
    }
}