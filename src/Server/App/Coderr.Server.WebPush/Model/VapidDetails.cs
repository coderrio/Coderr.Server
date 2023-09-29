using System;
using Coderr.Server.WebPush.Util;

namespace Coderr.Server.WebPush.Model
{
    public class VapidDetails
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private long _expiration = -1;
        private string _privateKey;
        private string _publicKey;
        private string _subject;
        public const string LiveSubject = "mailto:help@coderr.io";

        protected VapidDetails()
        {
        }

        /// <param name="subject">This should be a URL or a 'mailto:' email address</param>
        /// <param name="publicKey">The VAPID public key as a base64 encoded string</param>
        /// <param name="privateKey">The VAPID private key as a base64 encoded string</param>
        public VapidDetails(string subject, string publicKey, string privateKey)
        {
            Subject = subject;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        /// <summary>
        ///     Unix epoch based expiration. Default is 12 hours from now.
        /// </summary>
        public long Expiration
        {
            get => _expiration == -1 ? UnixTimeNow + 43200 : _expiration;
            set
            {
                if (value != -1 && value <= UnixTimeNow)
                    throw new ArgumentException(@"Vapid expiration must be a unix timestamp in the future");
                _expiration = value;
            }
        }

        /// <summary>
        ///     Must be a 32 bytes long key encoded as Base64
        /// </summary>
        public string PrivateKey
        {
            get => _privateKey;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException(@"Valid private key not set");

                var decodedPrivateKey = UrlBase64Helper.Decode(value);

                if (decodedPrivateKey.Length != 32)
                    throw new ArgumentException(@"Vapid private key should be 32 bytes long when decoded.");
                _privateKey = value;
            }
        }

        /// <summary>
        ///     Must be a 65 bytes long key encoded as Base64
        /// </summary>
        public string PublicKey
        {
            get => _publicKey;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException(@"Valid public key not set");

                var decodedPublicKey = UrlBase64Helper.Decode(value);

                if (decodedPublicKey.Length != 65)
                    throw new ArgumentException(@"Vapid public key must be 65 characters long when decoded");
                _publicKey = value;
            }
        }

        /// <summary>
        ///     This should be a URL or a 'mailto:' email address.
        /// </summary>
        public string Subject
        {
            get => _subject;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException(@"A subject is required");

                if (value.Length == 0)
                    throw new ArgumentException(
                        @"The subject value must be a string containing a url or mailto: address.");

                if (!value.StartsWith("mailto:") && !Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    throw new ArgumentException(@"Subject is not a valid URL or mailto address");

                _subject = value;
            }
        }

        private static long UnixTimeNow => (long)DateTime.UtcNow.Subtract(UnixEpoch).TotalSeconds;
    }
}