using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Reports;
using OneTrueError.App.Core.Reports;
using OneTrueError.App.Core.Reports.Invalid;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Reports
{
    [Component]
    internal class ErrorReportRepository : IReportsRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public ErrorReportRepository(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");

            _uow = uow;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
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

        public async Task<ReportDTO> GetAsync(int id)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE Id = @id";

                cmd.AddParameter("id", id);
                return await cmd.FirstOrDefaultAsync<ReportDTO>();
            }
        }

        public async Task<ReportDTO> FindByErrorIdAsync(string errorId)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE ErrorId = @id";

                cmd.AddParameter("id", errorId);
                return await cmd.FirstOrDefaultAsync<ReportDTO>();
            }
        }

        public async Task<PagedReports> GetForIncidentAsync(int incidentId, int pageNumber, int pageSize)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.AddParameter("incidentId", incidentId);
                long totalRows = 0;
                if (pageNumber > 0)
                {
                    cmd.CommandText =
                        "SELECT count(*) FROM ErrorReports WHERE IncidentId = @incidentId";
                    totalRows = (int) await cmd.ExecuteScalarAsync();
                }

                cmd.CommandText =
                    "SELECT * FROM ErrorReports WHERE IncidentId = @incidentId ORDER BY CreatedAtUtc DESC";
                if (pageNumber > 0)
                {
                    var offset = (pageNumber - 1)*pageSize;
                    cmd.CommandText += string.Format(@" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, pageSize);
                }

                //cmd.AddParameter("incidentId", incidentId);
                var list = await cmd.ToListAsync<ReportDTO>();
                return new PagedReports
                {
                    TotalCount = (int) totalRows,
                    Reports = (IReadOnlyList<ReportDTO>) list
                };
            }
        }

       
    }
}