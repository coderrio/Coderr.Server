using System;
using System.Collections.Generic;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Incidents;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.SqlServer.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests.Analysis
{
    public class IncidentBeingAnalyzedMapperTests : IntegrationTest
    {
        public IncidentBeingAnalyzedMapperTests(ITestOutputHelper helper) : base(helper)
        {
            helper.WriteLine("Hello world");
            ResetDatabase();
        }

        [Fact]
        public void Should_load_ignored_state_into_class_correctly()
        {
            var report = new ErrorReportEntity(FirstApplicationId, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                new ErrorReportException(new Exception("mofo")),
                new List<ErrorReportContext> {new ErrorReportContext("Maps", new Dictionary<string, string>())})
            {
                Title = "Missing here"
            };
            report.Init(report.GenerateHashCodeIdentifier());

            using (var uow = new AnalysisDbContext(CreateUnitOfWork()))
            {
                var incident = new IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(uow, new TestConfigStore());
                incRepos.CreateIncident(incident);
                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);
            }
        }
    }
}