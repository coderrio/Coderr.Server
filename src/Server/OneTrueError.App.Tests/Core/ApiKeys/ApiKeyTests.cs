using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OneTrueError.App.Core.ApiKeys;
using OneTrueError.Infrastructure.Security;
using Xunit;

namespace OneTrueError.App.Tests.Core.ApiKeys
{
    public class ApiKeyTests
    {
        [Fact]
        public void added_application_should_be_added_into_the_claim_list()
        {
            var sut = new ApiKey();

            sut.Add(1);

            var claim = sut.Claims.FirstOrDefault(x => x.Type == OneTrueClaims.Application && x.Value == "1");
            claim.Should().NotBeNull("because applications should be represented as claims");
        }

    }
}
