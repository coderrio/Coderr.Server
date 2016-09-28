using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.ApiKeys.Queries;
using OneTrueError.App.Core.ApiKeys;
using OneTrueError.SqlServer.Core.ApiKeys;
using OneTrueError.SqlServer.Core.ApiKeys.Queries;
using Xunit;

namespace OneTrueError.SqlServer.Tests.Core.ApiKeys.Queries
{
    public class ListApiKeysHandlerTests : IDisposable
    {
        private IAdoNetUnitOfWork _uow;
        private ApiKey _existingEntity;

        public ListApiKeysHandlerTests()
        {
            _existingEntity = new ApiKey
            {
                ApplicationName = "Arne",
                GeneratedKey = Guid.NewGuid().ToString("N"),
                SharedSecret = Guid.NewGuid().ToString("N"),
                CreatedById = 20,
                CreatedAtUtc = DateTime.UtcNow,
            };
            _existingEntity.Add(22);
            _uow = ConnectionFactory.Create();
            _uow.Insert(_existingEntity);
        }
        [Fact]
        public async void should_be_able_to_load_a_key()
        {
            var query = new ListApiKeys();

            var sut = new ListApiKeysHandler(_uow);
            var result = await sut.ExecuteAsync(query);

            result.Keys.Should().NotBeEmpty();
            result.Keys[0].ApiKey.Should().Be(_existingEntity.GeneratedKey);
            result.Keys[0].ApplicationName.Should().Be(_existingEntity.ApplicationName);
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
