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

namespace codeRR.Server.SqlServer.Tests.Core.ApiKeys.Queries
{
    [Collection(MapperInit.NAME)]
    public class ListApiKeysHandlerTests : IDisposable
    {
        private readonly ApiKey _existingEntity;
        private readonly IAdoNetUnitOfWork _uow;

        public ListApiKeysHandlerTests()
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
            _uow = ConnectionFactory.Create();
            _uow.Insert(_existingEntity);
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        [Fact]
        public async void should_be_able_to_load_a_key()
        {
            var query = new ListApiKeys();
            var context = Substitute.For<IMessageContext>();

            var sut = new ListApiKeysHandler(_uow);
            var result = await sut.HandleAsync(context, query);

            result.Keys.Should().NotBeEmpty();
            AssertionExtensions.Should((string) result.Keys[0].ApiKey).Be(_existingEntity.GeneratedKey);
            AssertionExtensions.Should((string) result.Keys[0].ApplicationName).Be(_existingEntity.ApplicationName);
        }
    }
}