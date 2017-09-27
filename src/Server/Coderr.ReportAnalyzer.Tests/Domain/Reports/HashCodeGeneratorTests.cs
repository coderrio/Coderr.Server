using System;
using System.Collections.Generic;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using FluentAssertions;
using Griffin.Container;
using NSubstitute;
using Xunit;

namespace codeRR.Server.ReportAnalyzer.Tests.Domain.Reports
{
    public class HashCodeGeneratorTests
    {
        [Fact]
        public void should_strip_line_numbers_to_reduce_the_number_of_incidents()
        {
            var locator = Substitute.For<IServiceLocator>();
            var ex1 = new ErrorReportException {StackTrace = @"   at System.Web.Compilation.AssemblyBuilder.Compile():line 64
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 65
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 67"};
            var ex2 = new ErrorReportException
            {
                StackTrace = @"   at System.Web.Compilation.AssemblyBuilder.Compile():line 12
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 23
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 53"
            };
            var report1 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex1, new List<ErrorReportContext>());
            var report2 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex2, new List<ErrorReportContext>());

            var sut = new HashCodeGenerator(locator);
            var actual1 = sut.GenerateHashCode(report1);
            var actual2 = sut.GenerateHashCode(report2);

            actual1.Should().Be(actual2);
        }

        [Fact]
        public void should_match_line_numbers_and_without_line_numbers_to_reduce_the_number_of_incidents()
        {
            var locator = Substitute.For<IServiceLocator>();
            var ex1 = new ErrorReportException { StackTrace = @"at System.Web.Compilation.AssemblyBuilder.Compile():line 64
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild():line 65
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath):line 67" };
            var ex2 = new ErrorReportException
            {
                StackTrace = @"at System.Web.Compilation.AssemblyBuilder.Compile()
at System.Web.Compilation.BuildProvidersCompiler.PerformBuild()
at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath)"
            };
            var report1 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex1, new List<ErrorReportContext>());
            var report2 = new ErrorReportEntity(1, "fjkkfjjkf", DateTime.UtcNow, ex2, new List<ErrorReportContext>());

            var sut = new HashCodeGenerator(locator);
            var actual1 = sut.GenerateHashCode(report1);
            var actual2 = sut.GenerateHashCode(report2);

            actual1.Should().Be(actual2);
        }
    }
}