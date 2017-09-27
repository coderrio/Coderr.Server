using System;
using System.Collections.Generic;
using System.Data;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.App.Core.Users;
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

        public void CreateDatabase()
        {
            using (var con = ConnectionFactory.OpenConnection())
            {
                _dbName = "T" + Guid.NewGuid().ToString("N");
                con.ExecuteNonQuery("CREATE Database " + _dbName);
                con.ChangeDatabase(_dbName);
                var schemaTool = new SchemaManager(() => con);
                schemaTool.CreateInitialStructure();
            }
        }

        public IDbConnection OpenConnection()
        {
            var connection = ConnectionFactory.OpenConnection();
            connection.ChangeDatabase(_dbName);
            return connection;
        }


        public void Dispose()
        {
            if (_dbName == null)
                return;

            using (var con = ConnectionFactory.OpenConnection())
            {
                var sql =
                    string.Format("alter database {0} set single_user with rollback immediate; DROP Database {0}",
                        _dbName);
                con.ExecuteNonQuery(sql);
            }
        }

        public IAdoNetUnitOfWork CreateUnitOfWork()
        {
            return new AdoNetUnitOfWork(OpenConnection(), false);
        }

        public void CreateUserAndApp()
        {
            using (var uow = CreateUnitOfWork())
            {
                var accountRepos = new AccountRepository(uow);
                var account = new Account("arne", "123456") {Email = "arne@som.com"};
                accountRepos.Create(account);
                var userRepos = new UserRepository(uow);
                var user = new User(account.Id, "arne") {EmailAddress = "arne@som.com"};
                userRepos.CreateAsync(user).GetAwaiter().GetResult();

                var appRepos = new ApplicationRepository(uow);
                var app = new Application(account.Id, "MinApp");
                appRepos.CreateAsync(app).GetAwaiter().GetResult();
                var member = new ApplicationTeamMember(app.Id, account.Id, "Admin");
                appRepos.CreateAsync(member).GetAwaiter().GetResult();


                var report = new ErrorReportEntity(app.Id, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                    new ErrorReportException(new Exception("mofo")),
                    new List<ErrorReportContext> {new ErrorReportContext("Maps", new Dictionary<string, string>())});
                report.Title = "Missing here";
                report.Init(report.GenerateHashCodeIdentifier());

                var incident = new ReportAnalyzer.Domain.Incidents.IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(uow);
                incRepos.CreateIncident(incident);
                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);

                uow.SaveChanges();
            }
        }

        public void ToLatestVersion()
        {
            var schemaTool = new SchemaManager(OpenConnection);
            schemaTool.UpgradeDatabaseSchema();
        }
    }
}
