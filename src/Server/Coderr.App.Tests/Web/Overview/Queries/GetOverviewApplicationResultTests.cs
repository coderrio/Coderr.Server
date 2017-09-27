using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using codeRR.Api.Web.Overview.Queries;
using Xunit;

namespace codeRR.App.Tests.Web.Overview.Queries
{
   public  class GetOverviewApplicationResultTests
    {

        [Fact]
        public void ignore_future_dates_to_allow_malconfigured_clients()
        {

            var sut = new GetOverviewApplicationResult("Label", DateTime.Today.AddDays(-30), 30);
            sut.AddValue(DateTime.Today.AddDays(1), 10);

        }

        [Fact]
        public void Should_allow_dates_within_the_given_interval()
        {

            var sut = new GetOverviewApplicationResult("hello", DateTime.Today.AddDays(-30), 31);
            sut.AddValue(DateTime.Today, 10);

            sut.Values[sut.Values.Length - 1].Should().Be(10);
        }
    }
}
