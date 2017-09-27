using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     Settings
    /// </summary>
    public class NotifyUsersActionSettings
    {
        /// <summary>
        ///     When to notify the users
        /// </summary>
        public NotifyActionType NotificationType { get; set; }

        /// <summary>
        ///     Can either be emails (contains '@') or account ids
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "I like my arrays.")]
        public string[] Targets { get; set; }
    }
}