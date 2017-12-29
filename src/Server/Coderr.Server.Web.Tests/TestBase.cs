using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Xunit;

namespace codeRR.Server.Web.Tests
{
    [TestCaseOrderer("codeRR.Server.Web.Tests.Helpers.xUnit.TestCaseOrderer", "codeRR.Server.Web.Tests")]
    public abstract class TestBase
    {
        private readonly IWebDriver _webDriver;

        protected TestBase(IWebDriver webDriver)
        {
            _webDriver = webDriver;
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
                var screenshot = _webDriver.TakeScreenshot();

                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{this.GetType().Name}.{DateTime.Now:yyyMMdd.HHmmss}.png");

                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);

                throw;
            }
        }
    }
}
