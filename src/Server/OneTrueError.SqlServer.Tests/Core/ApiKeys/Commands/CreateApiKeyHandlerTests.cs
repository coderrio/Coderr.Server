using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data;
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
        private IAdoNetUnitOfWork _uow;
        private int _applicationId;

        public CreateApiKeyHandlerTests()
        {
            _uow = ConnectionFactory.Create();
            GetApplicationId();
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

        [Fact]
        public async Task should_be_able_to_Create_key()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), new[] { _applicationId});

            var sut = new CreateApiKeyHandler(_uow);
            await sut.ExecuteAsync(cmd);

            var repos = new ApiKeyRepository(_uow);
            var generated = await repos.GetByKeyAsync(cmd.ApiKey);
            generated.Should().NotBeNull();
            generated.AllowedApplications.Should().BeEquivalentTo(new[] {_applicationId});
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
