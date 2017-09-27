using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using FluentAssertions;
using Griffin.Data;
using NSubstitute;
using codeRR.Api.Core.ApiKeys.Commands;
using codeRR.App.Core.Applications;
using codeRR.SqlServer.Core.ApiKeys;
using codeRR.SqlServer.Core.ApiKeys.Commands;
using codeRR.SqlServer.Core.Applications;
using Xunit;


namespace codeRR.SqlServer.Tests.Core.ApiKeys.Commands
{
    [Collection(MapperInit.NAME)]
    public class CreateApiKeyHandlerTests : IDisposable
    {
        private int _applicationId;
        private readonly IAdoNetUnitOfWork _uow;

        public CreateApiKeyHandlerTests()
        {
            _uow = ConnectionFactory.Create();
            GetApplicationId();
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        [Fact]
        public async Task Should_be_able_to_Create_a_key_without_applications_mapped()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
            var bus = Substitute.For<IEventBus>();

            var sut = new CreateApiKeyHandler(_uow, bus);
            await sut.ExecuteAsync(cmd);

            var repos = new ApiKeyRepository(_uow);
            var generated = await repos.GetByKeyAsync(cmd.ApiKey);
            generated.Should().NotBeNull();
            generated.Claims.Should().NotBeEmpty("because keys without appIds are universal");
        }

        [Fact]
        public async Task Should_be_able_to_Create_key()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"),
                new[] {_applicationId});
            var bus = Substitute.For<IEventBus>();

            var sut = new CreateApiKeyHandler(_uow, bus);
            await sut.ExecuteAsync(cmd);

            var repos = new ApiKeyRepository(_uow);
            var generated = await repos.GetByKeyAsync(cmd.ApiKey);
            generated.Should().NotBeNull();
            generated.Claims.First().Value.Should().BeEquivalentTo(_applicationId.ToString());
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