using System;
using codeRR.Server.Web.Tests.Helpers.Extensions;
using codeRR.Server.Web.Tests.Pages;
using OpenQA.Selenium;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class AdminUserTests : LoggedInTest, IDisposable
    {
        private readonly IPage _homePage;

        public AdminUserTests()
        {
            _homePage = Login();
        }

        [Fact]
        public void Admin_Should_have_create_new_application_menu_item()
        {
            UITest(() =>
            {
                Assert.IsType<HomePage>(_homePage);
                Assert.Equal("Overview", WebDriver.WaitUntilTitleEquals(_homePage.Title));
                Assert.NotNull(WebDriver.FindElement(By.XPath("//a/span[.='Create new application']")));
            });
        }
        
        public void Dispose()
        {
            Logout();
        }
    }
}
