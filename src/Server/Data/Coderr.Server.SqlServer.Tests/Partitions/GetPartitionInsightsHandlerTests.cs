using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Queries;
using Coderr.Server.Domain.Modules.Partitions;
using Coderr.Server.SqlServer.Partitions;
using DotNetCqs;
using Griffin.Data;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests.Partitions
{
    public class GetPartitionInsightsHandlerTests : CommonIntegrationTest
    {
        private readonly IAdoNetUnitOfWork _uow;
        private readonly PartitionsRepository _repository;

        [Fact]
        public async Task Should_be_able_to_make_query()
        {
            var appId = await FillDb();
            var context = Substitute.For<IMessageContext>();
            var msg = new GetPartitionInsights(appId);
            var repos = new PartitionsRepository(_uow);

            var sut = new GetPartitionInsightsHandler(_uow, repos);
            var result = await sut.HandleAsync(context, msg);

            
        }

        public GetPartitionInsightsHandlerTests(ITestOutputHelper output) : base(output)
        {
            ResetDatabase();

            _uow = CreateUnitOfWork();
            _repository = new PartitionsRepository(_uow);
        }

        protected async Task<int> FillDb()
        {
            var appId = base.CreateApplication("partitionTest1", FirstUserId);
            var def = new PartitionDefinition(appId, "Users2", "usrs2");
            await _repository.CreateAsync(def);
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Ada"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Key"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Doc"));
            _uow.ExecuteNonQuery($"INSERT INTO ApplicationPartitionInsights (PartitionId, Value, YearMonth) VALUES({def.Id}, 'Ada', '{DateTime.Today:yyyy-MM-01}')");
            _uow.SaveChanges();
            return appId;
        }

        protected override void Dispose(bool isBeingDisposed)
        {
            _uow.Dispose();
            base.Dispose(isBeingDisposed);
        }
    }
}
