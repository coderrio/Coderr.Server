using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Configuration;
using codeRR.Server.SqlServer.Core.Accounts;
using codeRR.Server.SqlServer.Tests.Helpers;
using codeRR.Server.SqlServer.Tests.Models;
using codeRR.Server.Web.Tests.Helpers;
using codeRR.Server.Web.Tests.Helpers.Selenium;
using Griffin.Data.Mapper;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace codeRR.Server.Web.Tests
{
    [TestCaseOrderer("codeRR.Server.Web.Tests.Helpers.xUnit.TestCaseOrderer", "codeRR.Server.Web.Tests")]
    public abstract class WebTest
    {
        private static readonly IisExpressHelper _iisExpress;
        private static readonly DatabaseManager _databaseManager = new DatabaseManager();

        static WebTest()
        {
            var mapper = new AssemblyScanningMappingProvider();
            mapper.Scan(typeof(AccountRepository).Assembly);
            EntityMappingProvider.Provider = mapper;

            _databaseManager.CreateEmptyDatabase();
            _databaseManager.InitSchema();

            AppDomain.CurrentDomain.DomainUnload += (o, e) =>
            {
                _iisExpress?.Stop();
                _databaseManager.Dispose();
            };

            // Disables database migration in codeRR.Server.Web project, should be up-to-date already
            // SchemaUpdateModule does not handle coderr_ConnectionString environment variable
            // This should only be run on build server due to changes in web.config
            if (Environment.GetEnvironmentVariable("TF_BUILD") != null)
            {
                DisableDatabaseMigrations();
            }

            var configPath =
                Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")),
                    "applicationhost.config");

            Console.WriteLine($"Path to IIS Express configuration file '{configPath}'");

            _iisExpress = new IisExpressHelper
            {
                ConfigPath = configPath,

                // Pass on connectionstring to codeRR.Server.Web during testing, overriding connectionstring in web.config
                EnvironmentVariables = new Dictionary<string, string> { { "coderr_ConnectionString", _databaseManager.ConnectionString } }
            };
            _iisExpress.Start("codeRR.Server.Web");

            // Warmup request only on build server
            if (Environment.GetEnvironmentVariable("TF_BUILD") != null)
            {
                var webClient = new WebClient();
                webClient.DownloadString(_iisExpress.BaseUrl);
            }

            TestUser = new TestUser {Username = "TestUser", Password = "123456", Email = "TestUser@coderrapp.com"};

            TestData = new TestDataManager(_databaseManager.OpenConnection) { TestUser = TestUser };

            WebDriver = DriverFactory.Create(BrowserType.Chrome);
            AppDomain.CurrentDomain.DomainUnload += (o, e) => { DisposeWebDriver(); };
        }

        /// <summary>
        /// Disables database migration in codeRR.Server.Web project
        /// </summary>
        private static void DisableDatabaseMigrations()
        {
            var webConfigFilename =
                Path.Combine(
                    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\codeRR.Server.Web\")),
                    "web.config");

            Console.WriteLine($"Setting DisableMigrations=true in '{webConfigFilename}'");

            // Prevent SchemaUpdateModule from running
            var configFile = new FileInfo(webConfigFilename);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            var configuration = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
            var appSettings = configuration.AppSettings.Settings;
            if (appSettings["DisableMigrations"] == null)
                appSettings.Add("DisableMigrations", "true");
            else
                appSettings["DisableMigrations"].Value = "true";
            configuration.Save();
        }

        protected WebTest()
        {
            TestData.ResetDatabase(_iisExpress.BaseUrl);
        }

        public string ServerUrl => _iisExpress.BaseUrl;

        public static TestDataManager TestData { get; }

        public static IWebDriver WebDriver { get; private set; }

        public static TestUser TestUser { get; set; }

        private static void DisposeWebDriver()
        {
            try
            {
                WebDriver.Quit();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            WebDriver.Dispose();
        }

        // ReSharper disable once InconsistentNaming
        public void UITest(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                var screenshot = WebDriver.TakeScreenshot();

                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    $"{GetType().Name}.{DateTime.Now:yyyMMdd.HHmmss}.png");

                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);

                throw;
            }
        }
    }
}