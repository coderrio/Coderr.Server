using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Modules.Onboarding.Queries;
using DotNetCqs;

namespace Coderr.Server.App.Modules.Onboarding.Queries
{
    internal class GetOnboardingStateHandler : IQueryHandler<GetOnboardingState, GetOnboardingStateResult>
    {
        private readonly IConfiguration<OnboardingSettings> _settings;

        public GetOnboardingStateHandler(IConfiguration<OnboardingSettings> settings)
        {
            _settings = settings;
        }

        public Task<GetOnboardingStateResult> HandleAsync(IMessageContext context, GetOnboardingState query)
        {
            return Task.FromResult(new GetOnboardingStateResult
            {
                IsComplete = _settings.Value.IsComplete,
                Libraries = _settings.Value.Libraries,
                MainLanguage = _settings.Value.MainLanguage
            });
        }
    }
}