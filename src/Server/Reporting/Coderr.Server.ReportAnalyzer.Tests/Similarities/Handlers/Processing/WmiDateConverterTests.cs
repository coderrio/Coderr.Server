using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Coderr.Server.ReportAnalyzer.Similarities.Handlers.Processing;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace Coderr.Server.ReportAnalyzer.Tests.Similarities.Handlers.Processing
{
    public class WmiDateConverterTests
    {
        [Fact]
        public void Test1()
        {
            //19th minus 8 hours for the timezone
            var expected = new DateTime(2009, 02, 18, 16, 0, 0);

            var success = WmiDateConverter.TryParse("20090219000000.000000+480", out var result);

            success.Should().BeTrue();
            result.Should().Be(expected);
        }

        [Fact]
        public void Test2()
        {
            var expected = new DateTime(2014, 04, 08, 15, 18, 35);
            expected = expected.AddMicroseconds(999999);

            var success = WmiDateConverter.TryParse("20140408141835.999999-60", out var result);

            success.Should().BeTrue();
            result.Should().Be(expected);
        }
    }
}
