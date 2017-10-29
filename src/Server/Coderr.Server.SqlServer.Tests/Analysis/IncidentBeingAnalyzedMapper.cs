using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Incidents;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.SqlServer.Analysis;
using Xunit;

namespace codeRR.Server.SqlServer.Tests.Analysis
{
    [Collection(MapperInit.NAME)]
    public class IncidentBeingAnalyzedMapperTests : IDisposable
    {
        private readonly TestTools _testTools = new TestTools();
        private int _accountId;
        private int _applicationId;

        public IncidentBeingAnalyzedMapperTests()
        {
            _testTools.CreateDatabase();
            _testTools.ToLatestVersion();
            _testTools.CreateUserAndApplication(out _accountId, out _applicationId);
        }
    
        [Fact]
        public void should_load_ignored_state_into_class_correctly()
        {
            var report = new ErrorReportEntity(_applicationId, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                new ErrorReportException(new Exception("mofo")),
                new List<ErrorReportContext> { new ErrorReportContext("Maps", new Dictionary<string, string>()) });
            report.Title = "Missing here";
            report.Init(report.GenerateHashCodeIdentifier());

            using (var uow = _testTools.CreateUnitOfWork())
            {
                var incident = new ReportAnalyzer.Domain.Incidents.IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(new AnalysisDbContext(uow), new TestConfigStore());
                incRepos.CreateIncident(incident);
                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);
            }

        }

        public void Dispose()
        {
            _testTools?.Dispose();
        }

    }
}
