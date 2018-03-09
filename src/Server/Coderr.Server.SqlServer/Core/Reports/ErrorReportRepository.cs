using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.Api.Core.Reports.Queries;
using codeRR.Server.App.Core.Reports;
using codeRR.Server.App.Core.Reports.Invalid;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Reports
{
    [Component]
    internal class ErrorReportRepository : IReportsRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public ErrorReportRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task CreateAsync(InvalidErrorReport entity)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO InvalidErrorReports (Id, AddedAtUtc, ApplicationId, Body, Exception) VALUES(@Id, @AddedAtUtc, @OrganizationId, @ApplicationId, @Body, @Exception)";
                cmd.AddParameter("Id", entity.Id);
                cmd.AddParameter("AddedAtUtc", entity.AddedAtUtc);
                cmd.AddParameter("ApplicationId", entity.ApplicationId);
                cmd.AddParameter("Body", entity.Report);
                cmd.AddParameter("Exception", entity.Exception);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<ReportDTO> GetAsync(int id)
        {
            ReportDTO report;
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE Id = @id";

                cmd.AddParameter("id", id);
                report = await cmd.FirstAsync<ReportDTO>();
            }

            var collections = new List<ContextCollectionDTO>();
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Name, PropertyName, Value FROM ErrorReportCollectionProperties WHERE ReportId = @id ORDER BY Name";

                cmd.AddParameter("id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    string collectionName = null;
                    var properties = new Dictionary<string, string>();
                    while (await reader.ReadAsync())
                    {
                        var name = reader.GetString(0);
                        if (collectionName == null)
                            collectionName = name;

                        if (collectionName != name)
                        {
                            var collection = new ContextCollectionDTO(collectionName, properties);
                            collections.Add(collection);
                            properties = new Dictionary<string, string>();
                            collectionName = name;
                        }

                        properties.Add(reader.GetString(1), reader.GetString(2));
                    }
                }
            }

            report.ContextCollections = collections.ToArray();
            return report;
        }

        public async Task<ReportDTO> FindByErrorIdAsync(string errorId)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE ErrorId = @id";

                cmd.AddParameter("id", errorId);
                return await cmd.FirstOrDefaultAsync<ReportDTO>();
            }
        }

        public async Task<PagedReports> GetForIncidentAsync(int incidentId, int pageNumber, int pageSize)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                cmd.AddParameter("incidentId", incidentId);
                long totalRows = 0;
                if (pageNumber > 0)
                {
                    cmd.CommandText =
                        "SELECT count(*) FROM ErrorReports WHERE IncidentId = @incidentId";
                    totalRows = (int)await cmd.ExecuteScalarAsync();
                }

                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE IncidentId = @incidentId ORDER BY CreatedAtUtc DESC";
                if (pageNumber > 0)
                {
                    var offset = (pageNumber - 1) * pageSize;
                    cmd.CommandText += string.Format(@" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, pageSize);
                }

                //cmd.AddParameter("incidentId", incidentId);
                var list = await cmd.ToListAsync<ReportDTO>();
                return new PagedReports
                {
                    TotalCount = (int)totalRows,
                    Reports = (IReadOnlyList<ReportDTO>)list
                };
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public IEnumerable<ReportDTO> GetAll(int[] ids)
        {
            //TODO: Remove SQL injection vulnerability

            using (var cmd = _uow.CreateCommand())
            {
                var idString = string.Join(",", ids.Select(x => "'" + x + "'"));

                cmd.CommandText =
                    string.Format(
                        "SELECT * FROM ErrorReports WHERE Id IN ({0})",
                        idString);

                return cmd.ToList<ReportDTO>().ToList();
            }
        }
    }
}