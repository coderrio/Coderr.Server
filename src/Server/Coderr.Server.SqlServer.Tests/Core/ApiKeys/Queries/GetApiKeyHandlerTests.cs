using System;
using codeRR.Server.Api.Core.ApiKeys.Queries;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.SqlServer.Core.ApiKeys;
using codeRR.Server.SqlServer.Core.ApiKeys.Queries;
using codeRR.Server.SqlServer.Core.Applications;
using DotNetCqs;
using FluentAssertions;
using Griffin.Data;
using NSubstitute;
using NSubstitute.Core;
using Xunit;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests.Core.ApiKeys.Queries
{

    public class GetApiKeyHandlerTests : IntegrationTest
    {
        private Application _application;
        private readonly ApiKey _existingEntity;

        public GetApiKeyHandlerTests(ITestOutputHelper helper):base(helper)
        {
            GetApplication();

            _existingEntity = new ApiKey
            {
                ApplicationName = "Arne",
                GeneratedKey = Guid.NewGuid().ToString("N"),
                SharedSecret = Guid.NewGuid().ToString("N"),
                CreatedById = 20,
                CreatedAtUtc = DateTime.UtcNow
            };

            _existingEntity.Add(_application.Id);
            using (var uow = CreateUnitOfWork())
            {
                var repos = new ApiKeyRepository(uow);
                repos.CreateAsync(_existingEntity).GetAwaiter().GetResult();
                uow.SaveChanges();
            }

        }



        [Fact]
        public async void Should_Be_able_to_fetch_existing_key_by_id()
        {
            var query = new GetApiKey(_existingEntity.Id);
            var context = Substitute.For<IMessageContext>();

            GetApiKeyResult result;
            using (var uow = CreateUnitOfWork())
            {
                var sut = new GetApiKeyHandler(uow);
                result = await sut.HandleAsync(context, query);
                uow.SaveChanges();
            }

            result.Should().NotBeNull();
            result.GeneratedKey.Should().Be(_existingEntity.GeneratedKey);
            result.ApplicationName.Should().Be(_existingEntity.ApplicationName);
            result.AllowedApplications[0].ApplicationId.Should().Be(_application.Id);
            result.AllowedApplications[0].ApplicationName.Should().Be(_application.Name);
        }

        private void GetApplication()
        {
            if (_application != null)
                return;

            using (var uow = CreateUnitOfWork())
            {
                var repos = new ApplicationRepository(uow);
                var id = uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications WITH (ReadPast)");
                if (id is DBNull || id is null)
                {
                    _application = new Application(10, "AppTen");
                    repos.CreateAsync(_application).GetAwaiter().GetResult();
                }
                else
                {
                    _application = repos.GetByIdAsync((int)id).Result;
                }
                uow.SaveChanges();
            }

        }
    }
}