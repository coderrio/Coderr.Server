using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.Tags;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.Tagging;
using Coderr.Server.ReportAnalyzer.Tagging.Handlers;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.Tagging
{
    public class IdentifyTagsFromIncidentTests
    {
        [Fact]
        public async Task should_be_able_to_identity_tags_when_only_one_is_specified()
        {
            var repos = Substitute.For<ITagsRepository>();
            var provider = Substitute.For<ITagIdentifierProvider>();
            provider.GetIdentifiers(Arg.Any<TagIdentifierContext>()).Returns(new ITagIdentifier[0]);
            var ctx = Substitute.For<IMessageContext>();
            var incident = new IncidentSummaryDTO(1, "Ada");
            var report = new ReportDTO
            {
                ContextCollections = new[]
                    {new ContextCollectionDTO("Data", new Dictionary<string, string> {{"ErrTags", "MyTag"}})}
            };
            var e = new ReportAddedToIncident(incident, report, false);

            var sut = new IdentifyTagsFromIncident(repos, provider);
            await sut.HandleAsync(ctx, e);

            var arguments = repos.ReceivedCalls().First(x => x.GetMethodInfo().Name == "AddAsync")
                .GetArguments();
            var tags = (Tag[]) arguments[1];
            tags[0].Name.Should().Be("MyTag");
        }

        [Fact]
        public async Task should_be_able_to_identity_tags_when_only_multiple_tags_are_specified()
        {
            var repos = Substitute.For<ITagsRepository>();
            var provider = Substitute.For<ITagIdentifierProvider>();
            provider.GetIdentifiers(Arg.Any<TagIdentifierContext>()).Returns(new ITagIdentifier[0]);
            var ctx = Substitute.For<IMessageContext>();
            var incident = new IncidentSummaryDTO(1, "Ada");
            var report = new ReportDTO
            {
                ContextCollections = new[]
                    {new ContextCollectionDTO("Data", new Dictionary<string, string> {{"ErrTags", "MyTag,YourTag"}})}
            };
            var e = new ReportAddedToIncident(incident, report, false);

            var sut = new IdentifyTagsFromIncident(repos, provider);
            await sut.HandleAsync(ctx, e);

            var arguments = repos.ReceivedCalls().First(x => x.GetMethodInfo().Name == "AddAsync")
                .GetArguments();
            var tags = (Tag[]) arguments[1];
            tags[0].Name.Should().Be("MyTag");
            tags[1].Name.Should().Be("YourTag");
        }

        [Fact]
        public async Task should_be_able_to_identity_tags_when_only_multiple_tags_are_specified_with_spaces()
        {
            var repos = Substitute.For<ITagsRepository>();
            var provider = Substitute.For<ITagIdentifierProvider>();
            provider.GetIdentifiers(Arg.Any<TagIdentifierContext>()).Returns(new ITagIdentifier[0]);
            var ctx = Substitute.For<IMessageContext>();
            var incident = new IncidentSummaryDTO(1, "Ada");
            var report = new ReportDTO
            {
                ContextCollections = new[]
                    {new ContextCollectionDTO("Data", new Dictionary<string, string> {{"ErrTags[]", "MyTag"}, {"ErrTags[]", "YourTag"}})}
            };
            var e = new ReportAddedToIncident(incident, report, false);

            var sut = new IdentifyTagsFromIncident(repos, provider);
            await sut.HandleAsync(ctx, e);

            var arguments = repos.ReceivedCalls().First(x => x.GetMethodInfo().Name == "AddAsync")
                .GetArguments();
            var tags = (Tag[]) arguments[1];
            tags[0].Name.Should().Be("MyTag");
            tags[1].Name.Should().Be("YourTag");
        }
    }
}