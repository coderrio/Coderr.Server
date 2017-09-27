using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.ApiKeys.Commands;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.SqlServer.Core.ApiKeys;
using codeRR.Server.SqlServer.Core.ApiKeys.Commands;
using codeRR.Server.SqlServer.Core.Applications;
using FluentAssertions;
using Griffin.Data;
using Xunit;

namespace codeRR.Server.SqlServer.Tests.Core.ApiKeys.Commands
{
    [Collection(MapperInit.NAME)]
    public class DeleteApiKeyHandlerTests : IDisposable
    {
        private int _applicationId;
        private readonly ApiKey _existingEntity;
        private readonly IAdoNetUnitOfWork _uow;

        public DeleteApiKeyHandlerTests()
        {
            _uow = ConnectionFactory.Create();
            GetApplicationId();

            _existingEntity = new ApiKey
            {
                ApplicationName = "Arne",
                GeneratedKey = Guid.NewGuid().ToString("N"),
                SharedSecret = Guid.NewGuid().ToString("N"),
                CreatedById = 20,
                CreatedAtUtc = DateTime.UtcNow
            };

            _existingEntity.Add(_applicationId);
            var repos = new ApiKeyRepository(_uow);
            repos.CreateAsync(_existingEntity).Wait();
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        [Fact]
        public async Task Should_be_able_to_delete_key_by_ApiKey()
        {
            var cmd = new DeleteApiKey(_existingEntity.GeneratedKey);

            var sut = new DeleteApiKeyHandler(_uow);
            await sut.ExecuteAsync(cmd);

            var count = _uow.ExecuteScalar("SELECT cast(count(*) as int) FROM ApiKeys WHERE Id = @id",
                new {id = _existingEntity.Id});
            count.Should().Be(0);
        }

        [Fact]
        public async Task Should_be_able_to_delete_key_by_id()
        {
            var cmd = new DeleteApiKey(_existingEntity.Id);

            var sut = new DeleteApiKeyHandler(_uow);
            await sut.ExecuteAsync(cmd);

            var count = _uow.ExecuteScalar("SELECT cast(count(*) as int) FROM ApiKeys WHERE Id = @id",
                new {id = _existingEntity.Id});
            count.Should().Be(0);
        }

        private void GetApplicationId()
        {
            var repos = new ApplicationRepository(_uow);
            var id = _uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications");
            if (id is DBNull || id is null)
            {
                repos.CreateAsync(new Application(10, "AppTen")).Wait();
                _applicationId = (int) _uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications");
            }
            else
                _applicationId = (int) id;
        }
    }
}