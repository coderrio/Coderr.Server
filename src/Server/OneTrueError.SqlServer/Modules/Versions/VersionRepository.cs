using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Versions;
using OneTrueError.App.Modules.Versions.Events;

namespace OneTrueError.SqlServer.Modules.Versions
{
    [Component]
    public class VersionRepository : IVersionRepository
    {
        private IAdoNetUnitOfWork _uow;

        public VersionRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task CreateAsync(ApplicationVersionMonth month)
        {
            await _uow.InsertAsync(month);
        }

        public async Task CreateAsync(ApplicationVersion entity)
        {
            await _uow.InsertAsync(entity);
        }

        public Task<ApplicationVersionMonth> GetMonthForApplicationAsync(int versionId, int year, int month)
        {
            var date = new DateTime(year, month, 1);
            return _uow.FirstOrDefaultAsync<ApplicationVersionMonth>(new { VersionId = versionId, YearMonth = date});

        }

        public Task<ApplicationVersion> GetVersionAsync(int applicationId, string version)
        {
            return _uow.FirstOrDefaultAsync<ApplicationVersion>(new { ApplicationId = applicationId, Version = version});
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
            var item = await _uow.FirstOrDefaultAsync<ApplicationVersionConfig>(new { ApplicationId = applicationId});
            return item?.AssemblyName;
        }
    }
}
