using System;
using System.Text;
using DotNetCqs;

namespace Coderr.Server.Api.Modules.Onboarding.Queries
{

    /// <summary>
    /// Get current state of the onboarding process.
    /// </summary>
    /// <remarks>The onboarding process is only run for the admin, and therefore these settings are site wide.</remarks>
    [Message]
    public class GetOnboardingState : Query<GetOnboardingStateResult>
    {
        
    }
}
