using System;

namespace Coderr.Server.WebPush.Model
{
    public class PushSubscription
    {
        protected PushSubscription()
        {
        }

        public PushSubscription(string endpoint, string p256dh, string auth)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            P256DH = p256dh ?? throw new ArgumentNullException(nameof(p256dh));
            Auth = auth;
        }

        public string Endpoint { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}