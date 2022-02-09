using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Modules.Onboarding.Commands;
using DotNetCqs;

namespace Coderr.Server.App.Modules.Onboarding.Commands
{
    class SetOnboardingChoicesHandler  : IMessageHandler<SetOnboardingChoices>
    {
        private IConfiguration<OnboardingSettings> _settings;

        public SetOnboardingChoicesHandler(IConfiguration<OnboardingSettings> settings)
        {
            _settings = settings;
        }

        public Task HandleAsync(IMessageContext context, SetOnboardingChoices message)
        {
            if (!string.IsNullOrEmpty(message.MainLanguage))
            {
                _settings.Value.MainLanguage = message.MainLanguage;
            }

            if (message.Libraries?.Any()==true)
            {
                _settings.Value.Libraries = message.Libraries;
                _settings.Value.IsComplete = true;
            }

            _settings.Save();

            return Task.CompletedTask;
        }
    }
}
