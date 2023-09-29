using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Mine.Queries;
using Coderr.Server.App.Modules.Mine;
using Coderr.Server.App.Partitions.Events;
using Coderr.Server.SqlServer.Core.Applications;
using Coderr.Server.SqlServer.Core.Incidents;
using Coderr.Server.SqlServer.Modules.Mine;
using Coderr.Server.SqlServer.Partitions;
using DotNetCqs;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests.Mine
{
    public class ListMyIncidentsQueryHandlerTests : CommonIntegrationTest
    {
        public ListMyIncidentsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Should_fetch_item()
        {
            var uow = CreateUnitOfWork();
            var repos = new PartitionsRepository(uow);
            var provider = new SuggestPartitionIncidents(repos);
            var service = new RecommendationService(new []{provider});
            var incidentRepository = new IncidentRepository(uow);
            var context = Substitute.For<IMessageContext>();
            var appRepos = new ApplicationRepository(uow);
            context.Principal.Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1"), })));

            var sut = new ListMyIncidentsQueryHandler(CreateUnitOfWork(), service, incidentRepository, appRepos);
            var query = new ListMyIncidents();
            await sut.HandleAsync(context, query);
        }

        //public async Task Should_return_result_even_when_there_are_no_suggestions_Available()
        //{

        //}

    }
}
