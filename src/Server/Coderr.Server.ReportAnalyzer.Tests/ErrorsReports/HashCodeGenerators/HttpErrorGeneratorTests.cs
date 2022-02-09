using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.ErrorReports.HashcodeGenerators;
using FluentAssertions;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.ErrorsReports.HashCodeGenerators
{
    public class HttpErrorGeneratorTests
    {
        [Fact]
        public void Should_remove_host_from_url()
        {
            var report = new ErrorReportEntity(1, "kffk", DateTime.UtcNow, new ErrorReportException(), new[]
            {
                new ErrorReportContextCollection("Yadayada",
                    new Dictionary<string, string>
                    {
                        {"Url", "http://localhost/some/path?hada=yada"},
                        {"HttpCode", "404" }
                    })
            });

            var sut = new HttpErrorGenerator();
            var code = sut.GenerateHashCode(report);

            var source = "404;/some/path";
            code.HashCode.Should().Be(HashCodeUtility.GetPersistentHashCode(source).ToString("X"));
        }

        [Fact]
        public void Should_remove_host_from_invalid_url()
        {
            var report = new ErrorReportEntity(1, "kffk", DateTime.UtcNow, new ErrorReportException(), new[]
            {
                new ErrorReportContextCollection("Yadayada",
                    new Dictionary<string, string>
                    {
                        {"Url", "http//localhost/some/path?hada=yada"},
                        {"HttpCode", "404" }
                    })
            });

            var sut = new HttpErrorGenerator();
            var code = sut.GenerateHashCode(report);

            var source = "404;http//localhost/some/path";
            code.HashCode.Should().Be(HashCodeUtility.GetPersistentHashCode(source).ToString("X"));
        }

        [Fact]
        public void Should_generate_from_relative_uri()
        {
            var report = new ErrorReportEntity(1, "kffk", DateTime.UtcNow, new ErrorReportException(), new[]
            {
                new ErrorReportContextCollection("Yadayada",
                    new Dictionary<string, string>
                    {
                        {"Url", "/some/path?hada=yada"},
                        {"HttpCode", "404" }
                    })
            });

            var sut = new HttpErrorGenerator();
            var code = sut.GenerateHashCode(report);

            var source = $"404;/some/path";
            code.HashCode.Should().Be(HashCodeUtility.GetPersistentHashCode(source).ToString("X"));
        }
    }
}
