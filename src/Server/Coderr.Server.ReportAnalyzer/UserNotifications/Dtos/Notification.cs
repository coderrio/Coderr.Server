using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coderr.Server.ReportAnalyzer.UserNotifications.Dtos
{
    /// <summary>
    ///     <see href="https://notifications.spec.whatwg.org/#dictdef-notificationoptions">Notification API Standard</see>
    /// </summary>
    public class Notification
    {
        public Notification()
        {
        }

        public Notification(string text)
        {
            Body = text;
        }

        [JsonProperty("actions")]
        public List<NotificationAction> Actions { get; set; } =
            new List<NotificationAction>();

        [JsonProperty("badge")] public string Badge { get; set; }

        [JsonProperty("body")] public string Body { get; set; }

        [JsonProperty("icon")] public string Icon { get; set; }

        [JsonProperty("image")] public string Image { get; set; }

        [JsonProperty("lang")] public string Lang { get; set; } = "en";

        [JsonProperty("requireInteraction")] public bool RequireInteraction { get; set; }

        [JsonProperty("tag")] public string Tag { get; set; }

        [JsonProperty("timestamp")] public DateTime Timestamp { get; set; } = DateTime.Now;

        [JsonProperty("title")] public string Title { get; set; } = "Push Demo";
    }
}