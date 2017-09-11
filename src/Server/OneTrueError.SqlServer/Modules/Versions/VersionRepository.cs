using System;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Versions;

namespace OneTrueError.SqlServer.Modules.Versions
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

        public async Task UpdateAsync(ApplicationVersionMonth month)
        {
            await _uow.UpdateAsync(month);
        }

        public async Task UpdateAsync(ApplicationVersion entity)
        {
            await _uow.UpdateAsync(entity);
        }


        public async Task<string> GetVersionAssemblyNameAsync(int applicationId)
        {
            var item = await _uow.FirstOrDefaultAsync<ApplicationVersionConfig>(new {ApplicationId = applicationId});
            return item?.AssemblyName;
        }
    }
}