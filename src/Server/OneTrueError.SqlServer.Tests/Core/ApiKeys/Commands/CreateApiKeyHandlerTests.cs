using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using FluentAssertions;
using Griffin.Data;
using NSubstitute;
using OneTrueError.Api.Core.ApiKeys.Commands;
using OneTrueError.App.Core.Applications;
using OneTrueError.SqlServer.Core.ApiKeys;
using OneTrueError.SqlServer.Core.ApiKeys.Commands;
using OneTrueError.SqlServer.Core.Applications;
using Xunit;

namespace OneTrueError.SqlServer.Tests.Core.ApiKeys.Commands
{
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
        public async Task should_be_able_to_Create_a_key_without_applications_mapped()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
            var bus = Substitute.For<IEventBus>();

            var sut = new CreateApiKeyHandler(_uow, bus);
            await sut.ExecuteAsync(cmd);

            var repos = new ApiKeyRepository(_uow);
            var generated = await repos.GetByKeyAsync(cmd.ApiKey);
            generated.Should().NotBeNull();
            generated.Claims.Should().BeEmpty();
        }

        [Fact]
        public async Task should_be_able_to_Create_key()
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
            if (id is DBNull)
            {
                repos.CreateAsync(new Application(10, "AppTen")).Wait();
                _applicationId = (int) _uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications");
            }
            else
                _applicationId = (int) id;
        }
    }
}