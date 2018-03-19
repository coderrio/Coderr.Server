using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Commands;
using Coderr.Server.Api.Core.Accounts.Events;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.App.Core.Accounts.CommandHandlers;
using Coderr.Server.Domain.Core.Account;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Server.App.Tests.Core.Accounts.CommandHandlers
{
    public class RegisterAccountHandlerTests
    {
        [Fact]
        public async Task Should_create_a_new_account()
        {
            var configStore = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            var context = Substitute.For<IMessageContext>();
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, configStore);
            await sut.HandleAsync(context, cmd);
            await repos.Received().CreateAsync(Arg.Any<Account>());
        }

        [Fact]
        public async Task Should_inform_the_rest_of_the_system_about_the_new_account()
        {
            var configStore = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var context = Substitute.For<IMessageContext>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, configStore);
            await sut.HandleAsync(context, cmd);

            await context.Received().SendAsync(Arg.Any<AccountRegistered>());
            context.Method("SendAsync").Arg<AccountRegistered>().AccountId.Should().Be(3);
        }

        [Fact]
        public async Task Should_send_activation_email()
        {
            var configStore = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var context = Substitute.For<IMessageContext>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, configStore);
            await sut.HandleAsync(context, cmd);

            await context.Received().SendAsync(Arg.Any<SendEmail>());
        }
    }
}