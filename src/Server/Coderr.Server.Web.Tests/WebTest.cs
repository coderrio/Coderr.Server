using System;
using System.Collections.Generic;
using System.IO;
using codeRR.Server.SqlServer.Core.Accounts;
using codeRR.Server.SqlServer.Tests.Helpers;
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

            TestData = new TestDataManager(_databaseManager.OpenConnection);
            WebDriver = DriverFactory.Create(BrowserType.Chrome);
            AppDomain.CurrentDomain.DomainUnload += (o, e) => { DisposeWebDriver(); };
        }

        protected WebTest()
        {
            TestData.ResetDatabase(_iisExpress.BaseUrl);
        }

        public string ServerUrl => _iisExpress.BaseUrl;

        public static TestDataManager TestData { get; }

        public static IWebDriver WebDriver { get; private set; }

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