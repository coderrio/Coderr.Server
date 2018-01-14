using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages.Account
{
    public class ActivationRequestedPage : BasePage
    {
        public ActivationRequestedPage(IWebDriver webDriver) : base(webDriver, (string) "Account/ActivationRequested", (string) "Account registered - codeRR")
        {
        }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }
    }
}
