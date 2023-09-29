using System;
using System.Collections.Generic;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Incidents;
using Coderr.Server.SqlServer.ReportAnalyzer;
using Xunit;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests.Analysis
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
                new List<ErrorReportContextCollection> {new ErrorReportContextCollection("Maps", new Dictionary<string, string>())})
            {
                Title = "Missing here"
            };
            report.Init(report.GenerateHashCodeIdentifier());

            using (var uow = CreateUnitOfWork())
            {
                var incident = new IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(uow);
                incRepos.CreateIncident(incident);
                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);
            }
        }
    }
}