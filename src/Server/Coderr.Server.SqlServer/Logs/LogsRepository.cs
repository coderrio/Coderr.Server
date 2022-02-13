using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.Logs;
using Griffin.Data;
using Newtonsoft.Json;

namespace Coderr.Server.Common.Data.SqlServer.Logs
{
    [ContainerService]
    public class LogsRepository : ILogsRepository
    {
        private IAdoNetUnitOfWork _unitOfWork;

        public LogsRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Exists(int incidentId, int? reportId)
        {
            if (reportId == null)
            {
                using (var cmd = _unitOfWork.CreateDbCommand())
                {
                    cmd.CommandText = "SELECT TOP(1) Id FROM ErrorReportLogs WHERE IncidentId = @id";
                    cmd.AddParameter("id", incidentId);
                    return await cmd.ExecuteScalarAsync() != DBNull.Value;
                }
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT TOP(1) Id FROM ErrorReportLogs WHERE IncidentId = @incidentId AND ReportId = @reportId";
                cmd.AddParameter("incidentId", incidentId);
                cmd.AddParameter("reportId", reportId);
                return await cmd.ExecuteScalarAsync() != null;
            }
        }

        public async Task<IReadOnlyList<LogEntry>> Get(int incidentId, int? reportId)
        {
            string json;
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT TOP(1) Json FROM ErrorReportLogs WHERE IncidentId = @incidentId";
                cmd.AddParameter("incidentId", incidentId);
                if (reportId != null)
                {
                    cmd.CommandText += " AND ReportId = @reportId";
                    cmd.AddParameter("reportId", reportId);
                }

                var obj = await cmd.ExecuteScalarAsync();
                if (!(obj is string s))
                {
                    return new LogEntry[0];
                }

                json = s;
            }

            return JsonConvert.DeserializeObject<LogEntry[]>(json);
        }

        public async Task Create(int incidentId, int reportId, IReadOnlyList<LogEntry> entries)
        {
            var json = JsonConvert.SerializeObject(entries);
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "INSERT INTO ErrorReportLogs (Json, IncidentId, ReportId) VALUES(@json, @incidentId, @errorReportId)";
                cmd.AddParameter("json", json);
                cmd.AddParameter("errorReportId", reportId);
                cmd.AddParameter("incidentId", incidentId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
