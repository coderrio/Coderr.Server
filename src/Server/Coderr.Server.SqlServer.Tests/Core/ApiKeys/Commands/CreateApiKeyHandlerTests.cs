using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.ApiKeys.Commands;
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

    public class CreateApiKeyHandlerTests : IntegrationTest
    {
        private int _applicationId;

        public CreateApiKeyHandlerTests(ITestOutputHelper helper) : base(helper)
        {
            GetApplicationId();
        }

        [Fact]
        public async Task Should_be_able_to_Create_a_key_without_applications_mapped()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
            var bus = Substitute.For<IMessageBus>();
            var ctx = Substitute.For<IMessageContext>();

            using (var uow = CreateUnitOfWork())
            {
                var sut = new CreateApiKeyHandler(uow, bus);
                await sut.HandleAsync(ctx, cmd);

                var repos = new ApiKeyRepository(uow);
                var generated = await repos.GetByKeyAsync(cmd.ApiKey);
                generated.Should().NotBeNull();
                generated.Claims.Should().NotBeEmpty("because keys without appIds are universal");

                uow.SaveChanges();
            }

        }

        [Fact]
        public async Task Should_be_able_to_Create_key()
        {
            var cmd = new CreateApiKey("Mofo", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"),
                new[] { _applicationId });
            var bus = Substitute.For<IMessageBus>();
            var ctx = Substitute.For<IMessageContext>();

            using (var uow = CreateUnitOfWork())
            {

                var sut = new CreateApiKeyHandler(uow, bus);
                await sut.HandleAsync(ctx, cmd);

                var repos = new ApiKeyRepository(uow);
                var generated = await repos.GetByKeyAsync(cmd.ApiKey);
                generated.Should().NotBeNull();
                generated.Claims.First().Value.Should().BeEquivalentTo(_applicationId.ToString());
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
                    repos.CreateAsync(new Application(10, "AppTen")).GetAwaiter().GetResult();
                    _applicationId = (int)uow.ExecuteScalar("SELECT TOP 1 Id FROM Applications WITH (ReadPast)");
                }
                else
                    _applicationId = (int)id;
                uow.SaveChanges();
            }

        }
    }
}