using codeRR.Server.Web.Tests.Helpers.Extensions;
using codeRR.Server.Web.Tests.Pages.Account;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class RegisterPageTests : LoggedInTest
    {
        [Fact]
        public void Should_not_be_able_to_register_using_already_taken_username()
        {
            UITest(() =>
            {
                var sut = new RegisterPage(WebDriver)
                    .RegisterUsingAlreadyTakenUsername();

                Assert.IsType<RegisterPage>(sut);
                sut.VerifyIsCurrentPage();

                sut.VerifyUsernameIsAlreadyTaken();
            });
        }

        [Fact]
        public void Should_not_be_able_to_register_using_already_taken_email()
        {
            UITest(() =>
            {
                var sut = new RegisterPage(WebDriver)
                    .RegisterUsingAlreadyTakenEmail();

                Assert.IsType<RegisterPage>(sut);
                sut.VerifyIsCurrentPage();

                sut.VerifyEmailIsAlreadyTaken();
            });
        }

        [Fact]
        public void Should_be_able_to_register_user()
        {
            UITest(() =>
            {
                var sut = new RegisterPage(WebDriver)
                    .RegisterNewUser();

                Assert.Equal("Account registered - codeRR", WebDriver.WaitUntilTitleEquals(sut.Title));
            });
        }
    }
}
