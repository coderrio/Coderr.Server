using Coderr.Server.ReportAnalyzer.ApplicationVersions.Handlers;
using FluentAssertions;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.ApplicationVersions.Handlers
{
    public class GetVersionFromReportTests
    {
        [Fact]
        public void Should_keep_one_zero_for_exact_majors()
        {
            var expected = "1.0";

            var actual = GetVersionFromReport.SimplifyVersion("1.0.0.0");

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_trim_end_zeros_but_leave_other_digits()
        {
            var expected = "3.1";

            var actual = GetVersionFromReport.SimplifyVersion("3.1.0.0");

            actual.Should().Be(expected);
        }
    }
}