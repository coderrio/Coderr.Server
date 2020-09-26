using System;
using System.Collections.Generic;
using System.Data;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.ReportAnalyzer.Incidents;
using Coderr.Server.SqlServer.Core.Accounts;
using Coderr.Server.SqlServer.Core.Applications;
using Coderr.Server.SqlServer.Core.Users;
using Coderr.Server.SqlServer.ReportAnalyzer;
using Coderr.Server.SqlServer.Tests.Models;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Tests.Helpers
{
    public class TestDataManager
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public TestDataManager(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            ConfigStore = new TestConfigStore();
            TestUser = new TestUser { Email = "demo@fortest.com", Password = "ada123", Username = "DemoTest" };
        }

        /// <summary>
        ///     Id of first user created in db
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The first user is automatically inserted when seeding DB with test data
        ///     </para>
        /// </remarks>
        public int AccountId { get; private set; }

        public TestUser TestUser { get; set; }

        public Application Application { get; private set; }

        /// <summary>
        ///     Id of first application created in db
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The first application is automatically inserted when seeding DB with test data, <see cref="AccountId" /> is
        ///         owner of app.
        ///     </para>
        /// </remarks>
        public int ApplicationId { get; private set; }

        public ConfigurationStore ConfigStore { get; private set; }

        public void ActivateAccount(int accountId)
        {
            using (var uow = CreateUnitOfWork())
            {
                var accountRepository = new AccountRepository(uow);
                var account = accountRepository.GetByIdAsync(accountId).GetAwaiter().GetResult();
                account.Activate();
                accountRepository.UpdateAsync(account).GetAwaiter().GetResult();

                uow.SaveChanges();
            }
        }

        /// <summary>
        ///     Creates an incident and a report.
        /// </summary>
        public void CreateReportAndIncident(out int reportId, out int incidentId)
        {
            ErrorReportEntity report;
            using (var uow = CreateUnitOfWork())
            {
                report = new ErrorReportEntity(ApplicationId, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                        new ErrorReportException(new Exception("mofo")),
                        new List<ErrorReportContextCollection>
                        {
                            new ErrorReportContextCollection("Maps", new Dictionary<string, string>())
                        })
                { Title = "Missing here" };
                report.Init(report.GenerateHashCodeIdentifier());

                uow.SaveChanges();
            }

            using (var dbContext = CreateUnitOfWork())
            {
                var incident = new IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(dbContext);
                incRepos.CreateIncident(incident);
                incidentId = incident.Id;

                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);
                reportId = report.Id;

                dbContext.SaveChanges();
            }


        }

        /// <summary>
        ///     Creates an incident and a report.
        /// </summary>
        public void CreateReportAndIncident(int applicationId, out int reportId, out int incidentId)
        {
            ErrorReportEntity report;
            using (var uow = CreateUnitOfWork())
            {
                report = new ErrorReportEntity(applicationId, Guid.NewGuid().ToString("N"), DateTime.UtcNow,
                        new ErrorReportException(new Exception("mofo")),
                        new List<ErrorReportContextCollection>
                        {
                            new ErrorReportContextCollection("Maps", new Dictionary<string, string>())
                        })
                { Title = "Missing here" };
                report.Init(report.GenerateHashCodeIdentifier());

                uow.SaveChanges();
            }

            using (var dbContext = CreateUnitOfWork())
            {
                var incident = new IncidentBeingAnalyzed(report);
                var incRepos = new AnalyticsRepository(dbContext);
                incRepos.CreateIncident(incident);
                incidentId = incident.Id;

                report.IncidentId = incident.Id;
                incRepos.CreateReport(report);
                reportId = report.Id;

                dbContext.SaveChanges();
            }


        }

        public void CreateUserAndApplication(out int accountId, out int applicationId)
        {
            using (var uow = CreateUnitOfWork())
            {
                CreateUserAndApplication(uow, out accountId, out applicationId);
                uow.SaveChanges();
            }
        }

        public void CreateUser(TestUser testUser, int applicationId)
        {
            using (var uow = CreateUnitOfWork())
            {
                var accountRepos = new AccountRepository(uow);
                var account = new Account(testUser.Username, testUser.Password) { Email = testUser.Email };
                account.Activate();
                accountRepos.Create(account);
                var userRepos = new UserRepository(uow);
                var user = new User(account.Id, testUser.Username) { EmailAddress = testUser.Email };
                userRepos.CreateAsync(user).GetAwaiter().GetResult();

                var appRepos = new ApplicationRepository(uow);
                var member = new ApplicationTeamMember(applicationId, account.Id, "Admin");
                appRepos.CreateAsync(member).GetAwaiter().GetResult();

                uow.SaveChanges();
            }
        }

        private void EnsureServerSettings(string baseUrl)
        {
            using (var con = _connectionFactory())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT Value FROM Settings WHERE Name = 'BaseUrl'";
                    var value = cmd.ExecuteScalar();
                    if (value != null)
                        return;
                }

                var sql = $@"INSERT INTO Settings (Section, Name, Value) VALUES
('BaseConfig', 'AllowRegistrations', 'True'), 
('BaseConfig', 'BaseUrl', '{baseUrl}'), 
('BaseConfig', 'SenderEmail', 'webtests@coderrapp.com'), 
('BaseConfig', 'SupportEmail', 'webtests@coderrapp.com'), 
('ErrorTracking', 'ActivateTracking', 'True'), 
('ErrorTracking', 'ContactEmail', 'webtests@coderrapp.com'), 
('ErrorTracking', 'InstallationId', '068e0fc19e90460c86526693488289ee'), 
('SmtpSettings', 'AccountName', ''), 
('SmtpSettings', 'AccountPassword', ''), 
('SmtpSettings', 'SmtpHost', 'localhost'), 
('SmtpSettings', 'PortNumber', '25'), 
('SmtpSettings', 'UseSSL', 'False')
";
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Application GetApplication(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                var repository = new ApplicationRepository(uow);
                return repository.GetByIdAsync(id).GetAwaiter().GetResult();
            }
        }

        public void ResetDatabase(string baseUrl)
        {
            EnsureServerSettings(baseUrl);
            using (var connection = _connectionFactory())
            {
                bool gotData;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Accounts WHERE Id = 1";
                    var value = cmd.ExecuteScalar();
                    gotData = value != null;
                }

                if (!gotData)
                {
                    CreateUserAndApplication(out var accountId, out var applicationId);
                    AccountId = accountId;
                    ApplicationId = applicationId;
                }
                else
                {
                    connection.ExecuteNonQuery("DELETE FROM Accounts WHERE ID > 1");
                    connection.ExecuteNonQuery("DELETE FROM Applications WHERE ID > 1");
                    connection.ExecuteNonQuery("DELETE FROM Incidents");
                    AccountId = 1;
                    ApplicationId = 1;
                }

                Application = GetApplication(ApplicationId);
            }
        }

        protected void CreateUserAndApplication(IAdoNetUnitOfWork uow, out int accountId, out int applicationId)
        {
            var accountRepos = new AccountRepository(uow);
            var account = new Account(TestUser.Username, TestUser.Password) { Email = TestUser.Email };
            account.Activate();
            accountRepos.Create(account);
            var userRepos = new UserRepository(uow);
            var user = new User(account.Id, TestUser.Username) { EmailAddress = TestUser.Email };
            userRepos.CreateAsync(user).GetAwaiter().GetResult();

            var appRepos = new ApplicationRepository(uow);
            var app = new Application(account.Id, "MyTestApp") { ApplicationType = TypeOfApplication.DesktopApplication };
            appRepos.CreateAsync(app).GetAwaiter().GetResult();
            var member = new ApplicationTeamMember(app.Id, account.Id, "Admin");
            appRepos.CreateAsync(member).GetAwaiter().GetResult();

            accountId = user.AccountId;
            applicationId = app.Id;
        }

        private IAdoNetUnitOfWork CreateUnitOfWork()
        {
            return new AdoNetUnitOfWork(_connectionFactory(), true);
        }

        public int CreateApplication(string name, int accountId)
        {
            using (var uow = CreateUnitOfWork())
            {
                var appRepos = new ApplicationRepository(uow);
                var app = new Application(accountId, "MyTestApp") { ApplicationType = TypeOfApplication.DesktopApplication };
                appRepos.CreateAsync(app).GetAwaiter().GetResult();
                var member = new ApplicationTeamMember(app.Id, accountId, "Admin");
                appRepos.CreateAsync(member).GetAwaiter().GetResult();

                uow.SaveChanges();
                return app.Id;
            }

        }
    }
}