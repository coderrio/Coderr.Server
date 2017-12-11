using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Incidents;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.ReportAnalyzer.Scanners;
using codeRR.Server.SqlServer.Tools;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.SqlServer.Analysis
{
    [Component]
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AnalyticsRepository));
        private readonly ReportDtoConverter _reportDtoConverter = new ReportDtoConverter();
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public AnalyticsRepository(AnalysisDbContext dbContext, ConfigurationStore configurationStore)
        {
            _unitOfWork = dbContext?.UnitOfWork ?? throw new ArgumentNullException(nameof(dbContext));
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

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO Incidents (ReportHashCode, ApplicationId, CreatedAtUtc, HashCodeIdentifier, StackTrace, ReportCount, UpdatedAtUtc, Description, FullName, IsReOpened, LastReportAtUtc)" +
                    " VALUES (@ReportHashCode, @ApplicationId, @CreatedAtUtc, @HashCodeIdentifier, @StackTrace, @ReportCount, @UpdatedAtUtc, @Description, @FullName, 0, @LastReportAtUtc);select SCOPE_IDENTITY();";
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
                cmd.AddParameter("LastReportAtUtc", incident.LastReportAtUtc);
                var id = (int) (decimal) cmd.ExecuteScalar();
                incident.GetType()
                    .GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(incident, id);
            }
            //_unitOfWork.Insert(incident);
        }

        public void UpdateIncident(IncidentBeingAnalyzed incident)
        {
            if (incident == null) throw new ArgumentNullException("incident");
            _unitOfWork.Update(incident);
        }

        public void CreateReport(ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException("report");

            if (string.IsNullOrEmpty(report.Title) && report.Exception != null)
            {
                report.Title = report.Exception.Message;
                if (report.Title.Length > 100)
                    report.Title = report.Title.Substring(0, 100);
            }


            _unitOfWork.Insert(report);
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
                                newReport.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

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