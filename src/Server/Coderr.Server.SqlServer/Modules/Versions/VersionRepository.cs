using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Versions;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Versions
{
    /// <summary>
    ///     ADO.NET based implementation of <see cref="IVersionRepository" />.
    /// </summary>
    [Component]
    public class VersionRepository : IVersionRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        /// <summary>
        ///     Creates a new instance of <see cref="VersionRepository" />
        /// </summary>
        /// <param name="uow">Unit of work</param>
        public VersionRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        /// <inheritdoc />
        public async Task CreateAsync(ApplicationVersionMonth month)
        {
            if (month == null) throw new ArgumentNullException(nameof(month));

            await _uow.InsertAsync(month);
        }

        /// <inheritdoc />
        public async Task CreateAsync(ApplicationVersion entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _uow.InsertAsync(entity);
        }


        /// <inheritdoc />
        public Task<ApplicationVersionMonth> FindMonthForApplicationAsync(int versionId, int year, int month)
        {
            if (versionId <= 0) throw new ArgumentOutOfRangeException(nameof(versionId));
            if (year <= 0) throw new ArgumentOutOfRangeException(nameof(year));
            if (month <= 0) throw new ArgumentOutOfRangeException(nameof(month));

            var date = new DateTime(year, month, 1);
            return _uow.FirstOrDefaultAsync<ApplicationVersionMonth>(new {VersionId = versionId, YearMonth = date});
        }

        /// <inheritdoc />
        public Task<ApplicationVersion> FindVersionAsync(int applicationId, string version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));

            return _uow.FirstOrDefaultAsync<ApplicationVersion>(new {ApplicationId = applicationId, Version = version});
        }

        public async Task<IList<ApplicationVersion>> FindForIncidentAsync(int incidentId)
        {
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = @"select ApplicationVersions.*
                                      FROM IncidentVersions
                                      JOIN ApplicationVersions ON (IncidentVersions.VersionId = ApplicationVersions.Id)
                                      WHERE IncidentVersions.IncidentId = @incidentId";
                cmd.AddParameter("incidentId", incidentId);
                return await cmd.ToListAsync<ApplicationVersion>();
            }
        }

        public void SaveIncidentVersion(int incidentId, int versionId)
        {
            var sql = @"INSERT INTO IncidentVersions (IncidentId, VersionId)
                        SELECT @incidentId, @versionId
                        WHERE NOT EXISTS (
                            select IncidentId
                              from IncidentVersions 
                              WHERE VersionId=@versionId
                        )";
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("incidentId", incidentId);
                cmd.AddParameter("versionId", versionId);
                cmd.ExecuteNonQuery();
            }
        }

        public async Task UpdateAsync(ApplicationVersionMonth month)
        {
            await _uow.UpdateAsync(month);
        }

        public async Task UpdateAsync(ApplicationVersion entity)
        {
            await _uow.UpdateAsync(entity);
        }

        public async Task<IEnumerable<string>> FindVersionsAsync(int appId)
        {
            return await _uow.ToListAsync<string>(new StringMapper(), "SELECT Version FROM ApplicationVersions WHERE ApplicationId = @id",
                new {id = appId});
        }


        public async Task<string> GetVersionAssemblyNameAsync(int applicationId)
        {
            var item = await _uow.FirstOrDefaultAsync<ApplicationVersionConfig>(new {ApplicationId = applicationId});
            return item?.AssemblyName;
        }
    }

    public class StringMapper : IEntityMapper<string>
    {
        public void Map(IDataRecord source, string destination)
        {
        }

        public object Create(IDataRecord record)
        {
            return record[0];
        }

        public void Map(IDataRecord source, object destination)
        {
        }
    }
}