using System;
using System.Collections.Generic;
using System.Data;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.App.Core.Users;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Incidents;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.SqlServer.Analysis;
using codeRR.Server.SqlServer.Core.Accounts;
using codeRR.Server.SqlServer.Core.Applications;
using codeRR.Server.SqlServer.Core.Users;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Tests
{
    public class TestTools : IDisposable
    {
        private string _dbName;

        public TestTools()
        {
            ConfigStore = new TestConfigStore();
            CanDropDatabase = true;
        }

        public ConfigurationStore ConfigStore { get; private set; }

        public bool CanDropDatabase { get; set; }

        public void Dispose()
        {
            if (_dbName == null)
                return;
            if (!CanDropDatabase)
            {
                Console.WriteLine("Do not delete " + _dbName);
                return;
            }

            using (var con = ConnectionFactory.OpenConnection())
            {
                var sql =
                    string.Format("alter database {0} set single_user with rollback immediate; DROP Database {0}",
                        _dbName);
                con.ExecuteNonQuery(sql);
            }
        }

        public void CreateBasicData()
        {
            using (var uow = CreateUnitOfWork())
            {
                CreateUserAndApplication(uow, out var accountId, out var applicationId);

                var report = new ErrorReportEntity(applicationId, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                    new ErrorReportException(new Exception("mofo")),
                    new List<ErrorReportContext> { new ErrorReportContext("Maps", new Dictionary<string, string>()) });
                report.Title = "Missing here";
                report.Init(report.GenerateHashCodeIdentifier());

                var incident = new IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(new AnalysisDbContext(uow), ConfigStore);
                incRepos.CreateIncident(incident);
                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);

                uow.SaveChanges();
            }
        }

        public void CreateDatabase()
        {
            using (var con = ConnectionFactory.OpenConnection())
            {
                _dbName = "CdrTest" + Guid.NewGuid().ToString("N").Substring(0, 8);
                con.ExecuteNonQuery("CREATE Database " + _dbName);
                con.ChangeDatabase(_dbName);
                var schemaTool = new SchemaManager(() => con);
                schemaTool.CreateInitialStructure();
            }
        }

        public IAdoNetUnitOfWork CreateUnitOfWork()
        {
            return new AdoNetUnitOfWork(OpenConnection(), false);
        }

        public void CreateUserAndApplication(out int accountId, out int applicationId)
        {
            using (var uow = CreateUnitOfWork())
            {
                CreateUserAndApplication(uow, out accountId, out applicationId);
                uow.SaveChanges();
            }
        }

        public IDbConnection OpenConnection()
        {
            var connection = ConnectionFactory.OpenConnection();
            connection.ChangeDatabase(_dbName);
            return connection;
        }

        public void ToLatestVersion()
        {
            var schemaTool = new SchemaManager(OpenConnection);
            schemaTool.UpgradeDatabaseSchema();
        }

        protected void CreateUserAndApplication(IAdoNetUnitOfWork uow, out int accountId, out int applicationId)
        {
            var accountRepos = new AccountRepository(uow);
            var account = new Account("arne", "123456") { Email = "arne@som.com" };
            accountRepos.Create(account);
            var userRepos = new UserRepository(uow);
            var user = new User(account.Id, "arne") { EmailAddress = "arne@som.com" };
            userRepos.CreateAsync(user).GetAwaiter().GetResult();

            var appRepos = new ApplicationRepository(uow);
            var app = new Application(account.Id, "MinApp");
            appRepos.CreateAsync(app).GetAwaiter().GetResult();
            var member = new ApplicationTeamMember(app.Id, account.Id, "Admin");
            appRepos.CreateAsync(member).GetAwaiter().GetResult();

            accountId = user.AccountId;
            applicationId = app.Id;
        }
    }
}