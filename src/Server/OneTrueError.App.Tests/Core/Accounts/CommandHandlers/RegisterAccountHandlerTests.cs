using System.Threading.Tasks;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using OneTrueError.Api.Core.Accounts.Commands;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Messaging.Commands;
using OneTrueError.App.Core.Accounts;
using OneTrueError.App.Core.Accounts.CommandHandlers;
using Xunit;

namespace OneTrueError.App.Tests.Core.Accounts.CommandHandlers
{
    public class RegisterAccountHandlerTests
    {
        [Fact]
        public async Task should_create_a_new_account()
        {
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "someEmal");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);
            await repos.Received().CreateAsync(Arg.Any<Account>());
        }

        [Fact]
        public async Task should_send_activation_email()
        {
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "someEmal");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);

            await cmdBus.Received().ExecuteAsync(Arg.Any<SendEmail>());
        }

        [Fact]
        public async Task should_inform_the_rest_of_the_system_about_the_new_account()
        {
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "someEmal");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);

            await eventBus.Received().PublishAsync(Arg.Any<AccountRegistered>());
            eventBus.Method("PublishAsync").Arg<AccountRegistered>().AccountId.Should().Be(3);
        }

    }
}