using System.Threading.Tasks;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using codeRR.Api.Core.Accounts.Commands;
using codeRR.Api.Core.Accounts.Events;
using codeRR.Api.Core.Messaging.Commands;
using codeRR.App.Core.Accounts;
using codeRR.App.Core.Accounts.CommandHandlers;
using codeRR.Infrastructure.Configuration;
using Xunit;

namespace codeRR.App.Tests.Core.Accounts.CommandHandlers
{
    public class RegisterAccountHandlerTests
    {
        [Fact]
        public async Task Should_create_a_new_account()
        {
            ConfigurationStore.Instance = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);
            await repos.Received().CreateAsync(Arg.Any<Account>());
        }

        [Fact]
        public async Task Should_inform_the_rest_of_the_system_about_the_new_account()
        {
            ConfigurationStore.Instance = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);

            await eventBus.Received().PublishAsync(Arg.Any<AccountRegistered>());
            eventBus.Method("PublishAsync").Arg<AccountRegistered>().AccountId.Should().Be(3);
        }

        [Fact]
        public async Task Should_send_activation_email()
        {
            ConfigurationStore.Instance = new TestStore();
            var repos = Substitute.For<IAccountRepository>();
            var cmdBus = Substitute.For<ICommandBus>();
            var eventBus = Substitute.For<IEventBus>();
            var cmd = new RegisterAccount("rne", "yo", "some@Emal.com");
            repos.When(x => x.CreateAsync(Arg.Any<Account>()))
                .Do(x => x.Arg<Account>().SetId(3));


            var sut = new RegisterAccountHandler(repos, cmdBus, eventBus);
            await sut.ExecuteAsync(cmd);

            await cmdBus.Received().ExecuteAsync(Arg.Any<SendEmail>());
        }
    }
}