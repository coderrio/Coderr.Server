using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.App.Modules.Geolocation;

namespace OneTrueError.SqlServer.Modules.Geolocation
{
    [Component]
    public class ErrorOriginRepository : IErrorOriginRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ErrorOriginRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(ErrorOrigin command, int applicationId, int incidentId, int reportId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT Id FROM ErrorOrigins WHERE IpAddress = @ip";
                cmd.AddParameter("ip", command.IpAddress);
                var id = await cmd.ExecuteScalarAsync();
                if (id is int)
                {
                    await CreateReportInfoAsync((int) id, applicationId, incidentId, reportId);
                    return;
                }
            }

            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ErrorOrigins (IpAddress, CountryCode, CountryName, RegionCode, RegionName, City, ZipCode, Latitude, Longitude, CreatedAtUtc) "
                                  +
                                  "VALUES (@IpAddress, @CountryCode, @CountryName, @RegionCode, @RegionName, @City, @ZipCode, @Latitude, @Longitude, @CreatedAtUtc);select cast(SCOPE_IDENTITY() as int);";
                cmd.AddParameter("IpAddress", command.IpAddress);
                cmd.AddParameter("CountryCode", command.CountryCode);
                cmd.AddParameter("CountryName", command.CountryName);
                cmd.AddParameter("RegionCode", command.RegionCode);
                cmd.AddParameter("RegionName", command.RegionName);
                cmd.AddParameter("City", command.City);
                cmd.AddParameter("ZipCode", command.ZipCode);
                //cmd.AddParameter("Point", SqlGeography.Point(command.Latitude, command.Longitude, 4326));
                cmd.AddParameter("Latitude", command.Latitude);
                cmd.AddParameter("Longitude", command.Longitude);
                cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);
                var id = (int) await cmd.ExecuteScalarAsync();
                await CreateReportInfoAsync(id, applicationId, incidentId, reportId);
            }
        }

        public async Task<IList<ErrorOrginListItem>> FindForIncidentAsync(int incidentId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT Longitude, Latitude, count(*) 
                                    FROM ErrorOrigins eo
                                    JOIN ErrorReportOrigins ON (eo.Id = ErrorReportOrigins.ErrorOriginId)
                                    WHERE IncidentId = @id
                                    GROUP BY IncidentId, Longitude, Latitude";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<ErrorOrginListItem>();
                    while (await reader.ReadAsync())
                    {
                        var item = new ErrorOrginListItem
                        {
                            Longitude = (double) reader.GetDecimal(0),
                            Latitude = (double) reader.GetDecimal(1),
                            NumberOfErrorReports = reader.GetInt32(2)
                        };
                        items.Add(item);
                    }
                    return items;
                }
            }
        }

        private async Task CreateReportInfoAsync(int originId, int applicationId, int incidentId, int reportId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO ErrorReportOrigins (ErrorOriginId, ApplicationId, IncidentId, ReportId, CreatedAtUtc) VALUES(@oid, @aid, @iid, @rid, GetUtcDate())";
                cmd.AddParameter("oid", originId);
                cmd.AddParameter("aid", applicationId);
                cmd.AddParameter("iid", incidentId);
                cmd.AddParameter("rid", reportId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}