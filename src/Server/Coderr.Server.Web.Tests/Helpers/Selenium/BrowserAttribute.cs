using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;
using Xunit.Sdk;

namespace codeRR.Server.Web.Tests.Helpers.Selenium
{
    public class BrowserAttribute : DataAttribute
    {
        private readonly IWebDriver _driver;

        public BrowserAttribute(BrowserType browserType)
        {
            _driver = DriverFactory.Create(browserType);
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return new[] {new[] {_driver}};
        }
    }
}
