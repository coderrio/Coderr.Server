using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages
{
    public class LoginPage : BasePage
    {
        private const string UserName = "arne";
        private const string Password = "123456";

        public LoginPage(IWebDriver webDriver) : base(webDriver, "Account/Login", "Login - codeRR")
        {
        }

        [FindsBy(How = How.Id, Using = "login-button")]
        public IWebElement SignInButton { get; set; }

        [FindsBy(How = How.Id, Using = "Username")]
        public IWebElement UserNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Password")]
        public IWebElement PasswordField { get; set; }

        public HomePage LoginWithValidCredentials()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(UserName);

            PasswordField.Clear();
            PasswordField.SendKeys(Password);

            SignInButton.Click();

            return new HomePage(WebDriver);
        }

        public LoginPage LoginWithNoUserNameSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();

            PasswordField.Clear();
            PasswordField.SendKeys(Password);

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithNoPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(UserName);

            PasswordField.Clear();

            SignInButton.Click();

            return this;
        }

        public LoginPage LoginWithWrongPasswordSpecified()
        {
            NavigateToPage();

            UserNameField.Clear();
            UserNameField.SendKeys(UserName);

            PasswordField.Clear();
            PasswordField.SendKeys(Password.Substring(1));

            SignInButton.Click();

            return this;
        }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }
    }
}
