using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.SqlServer.Tests;
using codeRR.Server.Web.Tests.Helpers;
using codeRR.Server.Web.Tests.Helpers.Selenium;
using OpenQA.Selenium;

namespace codeRR.Server.Web.Tests.Integration.Fixtures
{
    public class CommunityServerFixture : IDisposable
    {
        private readonly TestTools _testTools;

        public IWebDriver WebDriver { get; }
        public IisExpress IisExpress { get; }

        public Application Application { get; }
        public string BaseUrl => "http://localhost:50473/coderr/";

        public CommunityServerFixture()
        {
            var databaseName = $"coderrWebTest{DateTime.Now:yyyyMMddHHmmss}";
            var connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString.Replace("{databaseName}", databaseName);

            _testTools = new TestTools {CanDropDatabase = false};
            _testTools.CreateAndInitializeDatabase(AppDomain.CurrentDomain.BaseDirectory, databaseName, connectionString, BaseUrl);
            _testTools.CreateUserAndApplication(out int accountId, out int applicationId);
            _testTools.ActivateAccount(accountId);
            Application = _testTools.GetApplication(applicationId);

            IisExpress = new IisExpress
            {
                ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applicationhost.config"),
                EnvironmentVariables = new Dictionary<string, string>{ { "coderr_ConnectionString", connectionString } }
            };

            IisExpress.Start("codeRR.Server.Web");

            WebDriver = DriverFactory.Create(BrowserType.Chrome);
        }

        public void Dispose()
        {
            try
            {
                WebDriver.Quit();
                WebDriver.Close();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            IisExpress.Stop();

            _testTools.Dispose();
        }
    }
}
