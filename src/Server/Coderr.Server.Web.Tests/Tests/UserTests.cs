using System;
using codeRR.Server.SqlServer.Tests.Models;
using codeRR.Server.Web.Tests.Helpers.Extensions;
using codeRR.Server.Web.Tests.Pages;
using OpenQA.Selenium;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class UserTests : LoggedInTest, IDisposable
    {
        private readonly IPage _homePage;
        private readonly TestUser _testUser;

        public UserTests()
        {
            _testUser = new TestUser { Username = "TestNormalUser", Password = "123456", Email = "TestNormalUser@coderrapp.com" };
            TestData.CreateUser(_testUser, TestData.ApplicationId);

            _homePage = Login(_testUser.Username, _testUser.Password);
        }

        [Fact]
        public void NormalUser_Should_not_have_create_new_application_menu_item()
        {
            UITest(() =>
            {
                Assert.IsType<ApplicationPage>(_homePage);
                Assert.Equal("MyTestApp", WebDriver.WaitUntilTitleEquals("MyTestApp"));
                Assert.Throws<NoSuchElementException>(() => WebDriver.FindElement(By.XPath("//a/span[.='Create new application']")));
            });
        }

        [Fact]
        public void NormalUser_Should_see_homepage_after_login()
        {
            UITest(() =>
            {
                Assert.IsType<ApplicationPage>(_homePage);
                Assert.Equal("MyTestApp", WebDriver.WaitUntilTitleEquals("MyTestApp"));
            });
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
