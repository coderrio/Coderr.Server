using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.ErrorOrigins;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Geolocation
{
    [ContainerService]
    public class ErrorOriginRepository : IErrorOriginRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ErrorOriginRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(ErrorOrigin entity, int applicationId, int incidentId, int reportId)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            if (reportId <= 0) throw new ArgumentOutOfRangeException(nameof(reportId));

            if (entity.IpAddress != null)
            {
                using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM ErrorOrigins WHERE IpAddress = @ip";
                    cmd.AddParameter("ip", entity.IpAddress);
                    var id = await cmd.ExecuteScalarAsync();
                    if (id is int)
                    {
                        await CreateReportInfoAsync((int)id, applicationId, incidentId, reportId);
                        return;
                    }
                }
            }
            if (entity.Latitude < ErrorOrigin.EmptyLatitude && entity.Longitude < ErrorOrigin.EmptyLongitude)
            {
                using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM ErrorOrigins WHERE Longitude = @long AND Latitude = @lat";
                    cmd.AddParameter("long", entity.Longitude);
                    cmd.AddParameter("lat", entity.Latitude);
                    var id = await cmd.ExecuteScalarAsync();
                    if (id is int)
                    {
                        await CreateReportInfoAsync((int)id, applicationId, incidentId, reportId);
                        return;
                    }
                }
            }

            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ErrorOrigins (IpAddress, CountryCode, CountryName, RegionCode, RegionName, City, ZipCode, Latitude, Longitude, CreatedAtUtc, IsLookedUp) " +
                                  "VALUES (@IpAddress, @CountryCode, @CountryName, @RegionCode, @RegionName, @City, @ZipCode, @Latitude, @Longitude, @CreatedAtUtc, 0);select cast(SCOPE_IDENTITY() as int);";
                cmd.AddParameter("IpAddress", entity.IpAddress);
                cmd.AddParameter("CountryCode", entity.CountryCode);
                cmd.AddParameter("CountryName", entity.CountryName);
                cmd.AddParameter("RegionCode", entity.RegionCode);
                cmd.AddParameter("RegionName", entity.RegionName);
                cmd.AddParameter("City", entity.City);
                cmd.AddParameter("ZipCode", entity.ZipCode);
                //cmd.AddParameter("Point", SqlGeography.Point(command.Latitude, command.Longitude, 4326));
                cmd.AddParameter("Latitude", entity.Latitude);
                cmd.AddParameter("Longitude", entity.Longitude);
                cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);
                var id = (int)await cmd.ExecuteScalarAsync();
                entity.Id = id;
                await CreateReportInfoAsync(id, applicationId, incidentId, reportId);
            }
        }

        public Task<IList<ErrorOrigin>> GetPendingOrigins()
        {
            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT TOP(50) * FROM ErrorOrigins WHERE IsLookedUp = 0";
                return cmd.ToListAsync<ErrorOrigin>();
            }
        }

        public async Task Update(ErrorOrigin entity)
        {
            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText = "UPDATE ErrorOrigins SET " +
                                  "CountryCode=@CountryCode, " +
                                  "CountryName=@CountryName, " +
                                  "RegionCode=@RegionCode, " +
                                  "RegionName=@RegionName, " +
                                  "City=@City, " +
                                  "ZipCode=@ZipCode, " +
                                  "Latitude=@Latitude, " +
                                  "Longitude=@Longitude, " +
                                  "IsLookedUp = 1 " +
                                  "WHERE Id = @id";
                cmd.AddParameter("CountryCode", entity.CountryCode);
                cmd.AddParameter("CountryName", entity.CountryName);
                cmd.AddParameter("RegionCode", entity.RegionCode);
                cmd.AddParameter("RegionName", entity.RegionName);
                cmd.AddParameter("City", entity.City);
                cmd.AddParameter("ZipCode", entity.ZipCode);
                cmd.AddParameter("Latitude", entity.Latitude);
                cmd.AddParameter("Longitude", entity.Longitude);
                cmd.AddParameter("Id", entity.Id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IList<ErrorOriginListItem>> FindForIncidentAsync(int incidentId)
        {
            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT Longitude, Latitude, count(*) 
                                    FROM ErrorOrigins eo
                                    JOIN ErrorReportOrigins ON (eo.Id = ErrorReportOrigins.ErrorOriginId)
                                    WHERE IncidentId = @id AND IsLookedUp = 1
                                    GROUP BY IncidentId, Longitude, Latitude";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<ErrorOriginListItem>();
                    while (await reader.ReadAsync())
                    {
                        var item = new ErrorOriginListItem
                        {
                            Longitude = (double)reader.GetDecimal(0),
                            Latitude = (double)reader.GetDecimal(1),
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
            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
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