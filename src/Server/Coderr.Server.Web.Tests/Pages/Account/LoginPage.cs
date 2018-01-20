using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages.Account
{
    public class LoginPage : BasePage
    {
        public LoginPage(IWebDriver webDriver) : base(webDriver, "Account/Login", "Login - codeRR")
        {
        }

        [FindsBy(How = How.Id, Using = "login-button")]
        public IWebElement SignInButton { get; set; }

        [FindsBy(How = How.Id, Using = "Username")]
        public IWebElement UserNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Password")]
        public IWebElement PasswordField { get; set; }

        public IPage LoginWithValidCredentials(string userName, string password)
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(userName);

            PasswordField.Clear();
            PasswordField.SendKeys(password);

            SignInButton.Click();

            return PageHelper.ResolvePage(WebDriver);
        }

        public LoginPage LoginWithNonExistingUserWithoutPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys("NonExistingUsername");

            PasswordField.Clear();

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithNonExistingUserWithPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys("NonExistingUsername");

            PasswordField.Clear();
            PasswordField.SendKeys(TestUser.Password);

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithNoUserNameSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();

            PasswordField.Clear();
            PasswordField.SendKeys(TestUser.Password);

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithNoUserNameAndNoPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();

            PasswordField.Clear();

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithNoPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(TestUser.Username);

            PasswordField.Clear();

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithWrongPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(TestUser.Username);

            PasswordField.Clear();
            PasswordField.SendKeys(TestUser.Password.Substring(1));

            SignInButton.Click();

            return this;
        }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }
    }
}
