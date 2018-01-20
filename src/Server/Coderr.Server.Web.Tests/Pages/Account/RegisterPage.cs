using codeRR.Server.Web.Tests.Helpers.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages.Account
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver webDriver) : base(webDriver, "Account/Register", "Register account - codeRR")
        {
        }

        [FindsBy(How = How.Id, Using = "UserName")]
        public IWebElement UserNameField { get; set; }

        [FindsBy(How = How.Id, Using = "Password")]
        public IWebElement PasswordField { get; set; }

        [FindsBy(How = How.Id, Using = "Password2")]
        public IWebElement RetypePasswordField { get; set; }

        [FindsBy(How = How.Id, Using = "Email")]
        public IWebElement EmailField { get; set; }

        [FindsBy(How = How.ClassName, Using = "btn-primary")]
        public IWebElement SignUpButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "field-validation-error")]
        public IWebElement ValidationErrorField { get; set; }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }

        public RegisterPage RegisterUsingAlreadyTakenUsername()
        {
            NavigateToPage();

            ClearForm();

            UserNameField.SendKeys(TestUser.Username);
            PasswordField.SendKeys(TestUser.Password);
            RetypePasswordField.SendKeys(TestUser.Password);
            EmailField.SendKeys(TestUser.Email);

            SignUpButton.Click();

            return this;
        }

        public RegisterPage RegisterUsingAlreadyTakenEmail()
        {
            NavigateToPage();

            ClearForm();

            UserNameField.SendKeys(TestUser.Username + "2");
            PasswordField.SendKeys(TestUser.Password);
            RetypePasswordField.SendKeys(TestUser.Password);
            EmailField.SendKeys(TestUser.Email);

            SignUpButton.Click();

            return this;
        }

        public ActivationRequestedPage RegisterNewUser()
        {
            NavigateToPage();

            ClearForm();

            UserNameField.SendKeys(TestUser.Username + "2");
            PasswordField.SendKeys(TestUser.Password);
            RetypePasswordField.SendKeys(TestUser.Password);
            EmailField.SendKeys("TestUser2@coderrapp.com");

            SignUpButton.Click();

            return new ActivationRequestedPage(WebDriver);
        }

        public RegisterPage VerifyUsernameIsAlreadyTaken()
        {
            WebDriver.WaitUntilElementIsPresent(ValidationErrorField);
            return this;
        }

        public RegisterPage VerifyEmailIsAlreadyTaken()
        {
            WebDriver.WaitUntilElementIsPresent(ValidationErrorField);
            return this;
        }

        private void ClearForm()
        {
            UserNameField.Clear();
            PasswordField.Clear();
            RetypePasswordField.Clear();
            EmailField.Clear();
        }
    }
}
