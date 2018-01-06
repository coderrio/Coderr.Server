using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.ApiKeys.Commands;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.SqlServer.Core.ApiKeys;
using codeRR.Server.SqlServer.Core.ApiKeys.Commands;
using codeRR.Server.SqlServer.Core.Applications;
using DotNetCqs;
using FluentAssertions;
using Griffin.Data;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests.Core.ApiKeys.Commands
{

    public class DeleteApiKeyHandlerTests : IntegrationTest
    {
        private int _applicationId;
        private readonly ApiKey _existingEntity;

        public DeleteApiKeyHandlerTests(ITestOutputHelper helper) : base(helper)
        {
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
            using (var uow = CreateUnitOfWork())
            {
                var repos = new ApiKeyRepository(uow);
                repos.CreateAsync(_existingEntity).GetAwaiter().GetResult();
                uow.SaveChanges();
            }
        }
        [Fact]
        public async Task Should_be_able_to_delete_key_by_ApiKey()
        {
            var cmd = new DeleteApiKey(_existingEntity.GeneratedKey);
            var context = Substitute.For<IMessageContext>();

            using (var uow = CreateUnitOfWork())
            {
                var sut = new DeleteApiKeyHandler(uow);
                await sut.HandleAsync(context, cmd);

                var count = uow.ExecuteScalar("SELECT cast(count(*) as int) FROM ApiKeys WHERE Id = @id",
                    new { id = _existingEntity.Id });
                count.Should().Be(0);
                uow.SaveChanges();
            }

        }

        [Fact]
        public async Task Should_be_able_to_delete_key_by_id()
        {
            var cmd = new DeleteApiKey(_existingEntity.Id);
            var context = Substitute.For<IMessageContext>();

            using (var uow = CreateUnitOfWork())
            {
                var sut = new DeleteApiKeyHandler(uow);
                await sut.HandleAsync(context, cmd);

                var count = uow.ExecuteScalar("SELECT cast(count(*) as int) FROM ApiKeys WHERE Id = @id",
                    new { id = _existingEntity.Id });
                count.Should().Be(0);
                uow.SaveChanges();
            }

        }

        private void GetApplicationId()
        {
            if (_applicationId != 0)
                return;

            using (var uow = CreateUnitOfWork())
            {
                var repos = new ApplicationRepository(uow);
                var id = uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications WITH (ReadPast)");
                if (id is DBNull || id is null)
                {
                    repos.CreateAsync(new Application(10, "AppTen")).Wait();
                    _applicationId = (int)uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications WITH (ReadPast)");
                }
                else
                    _applicationId = (int)id;
                uow.SaveChanges();
            }

        }
    }
}