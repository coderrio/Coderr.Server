using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.ErrorOrigins.Handlers;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.ErrorOrigins.Handlers
{
    public class StorePositionFromNewReportTests
    {
        [Fact]
        public async Task Should_be_able_to_use_position_in_CoderrCollection()
        {
            var repos = Substitute.For<IErrorOriginRepository>();
            var incident = new IncidentSummaryDTO(1, "Hello");
            var data = new Dictionary<string, string> {{"Longitude", "60.6065"}, {"Latitude", "15.6355"}};
            var report = new ReportDTO {ContextCollections = new[] {new ContextCollectionDTO("CoderrData", data)}};
            var e = new ReportAddedToIncident(incident, report, false);
            var context = Substitute.For<IMessageContext>();
            var configWrapper = Substitute.For<IConfiguration<OriginsConfiguration>>();

            var sut = new StorePositionFromNewReport(repos, configWrapper);
            await sut.HandleAsync(context, e);

            var entity = (ErrorOrigin)repos.ReceivedCalls().First(x => x.GetMethodInfo().Name == "CreateAsync")
                .GetArguments().First();
            entity.Latitude.Should().Be(15.6355);
            entity.Longitude.Should().Be(60.6065);
        }

        [Fact]
        public async Task Should_be_able_to_use_position_in_regular_collection()
        {
            var repos = Substitute.For<IErrorOriginRepository>();
            var incident = new IncidentSummaryDTO(1, "Hello");
            var data = new Dictionary<string, string> {{"ReportLongitude", "60.6065"}, {"ReportLatitude", "15.6355"}};
            var report = new ReportDTO {ContextCollections = new[] {new ContextCollectionDTO("SomeCollection", data)}};
            var e = new ReportAddedToIncident(incident, report, false);
            var context = Substitute.For<IMessageContext>();
            var configWrapper = Substitute.For<IConfiguration<OriginsConfiguration>>();

            var sut = new StorePositionFromNewReport(repos, configWrapper);
            await sut.HandleAsync(context, e);

            var entity = (ErrorOrigin)repos.ReceivedCalls().First(x => x.GetMethodInfo().Name == "CreateAsync")
                .GetArguments().First();
            entity.Latitude.Should().Be(15.6355);
            entity.Longitude.Should().Be(60.6065);
        }
    }
}