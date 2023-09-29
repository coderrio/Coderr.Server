using System;
using Newtonsoft.Json;

namespace Coderr.Server.WebPush.Model
{
    /// <summary>
    ///     <see href="https://notifications.spec.whatwg.org/#dictdef-notificationaction">Notification API Standard</see>
    /// </summary>
    public class NotificationAction
    {
        public NotificationAction(string action, string title)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }

        /// <summary>
        /// Action in the service worker.
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Title shown for the action.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; private set; }
    }
}