using System;

namespace Coderr.Server.Domain.Modules.UserNotifications
{
    public class BrowserSubscription
    {
        public int AccountId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// </summary>
        public string AuthenticationSecret { get; set; }

        public string Endpoint { get; set; }

        public DateTime? ExpiresAtUtc { get; set; }

        public int Id { get; set; }

        /// <summary>
        ///     https://developer.mozilla.org/en-US/docs/Web/API/PushSubscription/getKey
        /// </summary>
        public string PublicKey { get; set; }
    }
}