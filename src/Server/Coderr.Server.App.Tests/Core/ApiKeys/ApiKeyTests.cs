using System.Linq;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.Infrastructure.Security;
using FluentAssertions;
using Xunit;

namespace codeRR.Server.App.Tests.Core.ApiKeys
{
    public class ApiKeyTests
    {
        [Fact]
        public void added_application_should_be_added_into_the_claim_list()
        {
            var sut = new ApiKey();

            sut.Add(1);

            var claim = sut.Claims.FirstOrDefault(x => x.Type == CoderrClaims.Application && x.Value == "1");
            claim.Should().NotBeNull("because applications Should be represented as claims");
        }

    }
}
