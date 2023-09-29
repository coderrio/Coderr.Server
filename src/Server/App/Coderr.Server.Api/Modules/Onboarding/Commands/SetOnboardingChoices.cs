using System.Collections.Generic;

namespace Coderr.Server.Api.Modules.Onboarding.Commands
{
    /// <summary>
    /// Set current state of the onboarding.
    /// </summary>
    [Command]
    public class SetOnboardingChoices
    {
        /// <summary>
        /// All libraries that the user wants to generate example incidents for.
        /// </summary>
        public IReadOnlyList<string> Libraries { get; set; }

        /// <summary>
        /// DOTNET or NODEJS
        /// </summary>
        public string MainLanguage { get; set; }

        public string Feedback { get; set; }
    }
}