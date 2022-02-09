
using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.App.Modules.Onboarding
{
    /// <summary>
    /// Current state of onboarding.
    /// </summary>
    public class OnboardingSettings : IConfigurationSection
    {
        public bool IsComplete { get; set; }
        public IReadOnlyList<string> Libraries { get; set; }


        /// <summary>
        ///     DOTNET or NODEJS
        /// </summary>
        public string MainLanguage { get; set; }

        public string SectionName { get; } = "Onboarding";

        public string Feedback { get; set; }

        public void Load(IDictionary<string, string> settings)
        {
            if (settings.TryGetValue("MainLanguage", out var lang)) MainLanguage = lang;
            if (settings.TryGetValue("Feedback", out var value)) Feedback = value;
            if (settings.TryGetValue("IsComplete", out var complete)) IsComplete = complete == true.ToString();
            if (settings.TryGetValue("Libraries", out var libs)) Libraries = libs.Split(',');
        }

        public IDictionary<string, string> ToDictionary()
        {
            var items = new Dictionary<string, string>
            {
                ["IsComplete"] = IsComplete.ToString(),
                [nameof(MainLanguage)] = MainLanguage,
                [nameof(Libraries)] = Libraries == null ? "" : string.Join(",", Libraries),
                [nameof(Feedback)] = Feedback
            };
            return items;
        }
    }
}