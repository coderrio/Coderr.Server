using System.Collections.Generic;

namespace Coderr.Server.Api.Modules.Onboarding.Queries
{
    /// <summary>
    /// Result for <see cref="GetOnboardingStateResult"/>
    /// </summary>
    public class GetOnboardingStateResult
    {
        /// <summary>
        /// Onboarding is completed.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Libraries that the user generated demo incidents for.
        /// </summary>
        public IReadOnlyList<string> Libraries { get; set; }

        /// <summary>
        /// DOTNET or NODEJS
        /// </summary>
        public string MainLanguage { get; set; }

    }
}