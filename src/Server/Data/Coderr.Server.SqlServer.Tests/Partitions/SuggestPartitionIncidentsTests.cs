using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.App.Modules.Mine;
using Coderr.Server.App.Partitions.Events;
using Coderr.Server.Domain.Modules.Partitions;
using Coderr.Server.SqlServer.Partitions;
using FluentAssertions;
using Griffin.Data;
using Xunit;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests.Partitions
{
    public class SuggestPartitionIncidentsTests : CommonIntegrationTest
    {
        private PartitionsRepository _repository;
        private int _reportId;
        private int _incidentId;
        private IAdoNetUnitOfWork _uow;

        public SuggestPartitionIncidentsTests(ITestOutputHelper output) : base(output)
        {
            _uow = CreateUnitOfWork();
            _repository = new PartitionsRepository(_uow);
            ResetDatabase();
        }

        [Fact]
        public async Task Should_calculate_using_NumberOfItems_when_specified()
        {
            var appId = base.CreateApplication("partitionTest1", FirstUserId);
            CreateReportAndIncident(appId, out _reportId, out _incidentId);
            var items = new List<RecommendedIncident>();
            var ctx = new RecommendIncidentContext(items) { ApplicationId = appId };
            var def = new PartitionDefinition(appId, "Users1", "usrs1")
            {
                NumberOfItems = 100,
            };
            await _repository.CreateAsync(def);
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Ada"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Key"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Doc"));
            await _repository.CreateAsync(new IncidentPartitionValue(def.Id, _incidentId, 1));
            await _repository.CreateAsync(new IncidentPartitionValue(def.Id, _incidentId, 2));
            _uow.SaveChanges();

            var sut = new SuggestPartitionIncidents(_repository);
            await sut.Recommend(ctx);

            items.Should().NotBeEmpty();
            items[0].Score.Should().Be(2, "because 2 of 100 is 2.");
        }

        [Fact]
        public async Task Should_calculate_using_AppCount_when_NumberOfItems_is_not_specified()
        {
            var appId = base.CreateApplication("partitionTest1", FirstUserId);
            CreateReportAndIncident(appId, out _reportId, out _incidentId);
            var items = new List<RecommendedIncident>();
            var ctx = new RecommendIncidentContext(items) { ApplicationId = appId };
            var def = new PartitionDefinition(appId, "Users2", "usrs2");
            await _repository.CreateAsync(def);
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Ada"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Key"));
            await _repository.CreateAsync(new ApplicationPartitionValue(def.Id, "Doc"));
            await _repository.CreateAsync(new IncidentPartitionValue(def.Id, _incidentId, 1));
            await _repository.CreateAsync(new IncidentPartitionValue(def.Id, _incidentId, 2));
            _uow.SaveChanges();

            var sut = new SuggestPartitionIncidents(_repository);
            await sut.Recommend(ctx);

            items.Should().NotBeEmpty();
            items[0].Score.Should().Be(66, "because 2 of 3 is 66%.");
        }
    }
}
