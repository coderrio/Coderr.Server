using System;
using codeRR.Server.Api.Core.ApiKeys.Queries;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.SqlServer.Core.ApiKeys.Queries;
using DotNetCqs;
using FluentAssertions;
using Griffin.Data;
using Griffin.Data.Mapper;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests.Core.ApiKeys.Queries
{

    public class ListApiKeysHandlerTests : IntegrationTest
    {
        private readonly ApiKey _existingEntity;

        public ListApiKeysHandlerTests(ITestOutputHelper helper) : base(helper)
        {
            _existingEntity = new ApiKey
            {
                ApplicationName = "Arne",
                GeneratedKey = Guid.NewGuid().ToString("N"),
                SharedSecret = Guid.NewGuid().ToString("N"),
                CreatedById = 20,
                CreatedAtUtc = DateTime.UtcNow
            };
            _existingEntity.Add(22);
            using (var uow = CreateUnitOfWork())
            {
                uow.Insert(_existingEntity);
                uow.SaveChanges();
            }

        }


        [Fact]
        public async void Should_be_able_to_load_a_key()
        {
            var query = new ListApiKeys();
            var context = Substitute.For<IMessageContext>();

            ListApiKeysResult result;
            using (var uow = CreateUnitOfWork())
            {
                var sut = new ListApiKeysHandler(uow);
                result = await sut.HandleAsync(context, query);
                uow.SaveChanges();

            }

            result.Keys.Should().NotBeEmpty();
            AssertionExtensions.Should((string)result.Keys[0].ApiKey).Be(_existingEntity.GeneratedKey);
            AssertionExtensions.Should((string)result.Keys[0].ApplicationName).Be(_existingEntity.ApplicationName);
        }
    }
}