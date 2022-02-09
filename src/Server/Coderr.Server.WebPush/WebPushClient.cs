using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Coderr.Server.WebPush.Model;
using Coderr.Server.WebPush.Util;
using Newtonsoft.Json;

namespace Coderr.Server.WebPush
{
    public class WebPushClient : IDisposable
    {
        // default TTL is 4 weeks.
        private const int DefaultTtl = 2419200;

        private string _gcmApiKey;
        private HttpClient _httpClient = new HttpClient();
        private VapidDetails _vapidDetails;

        public WebPushClient(VapidDetails vapidDetails)
        {
            _vapidDetails = vapidDetails ?? throw new ArgumentNullException(nameof(vapidDetails));
        }

        public WebPushClient(string gcmApiKey)
        {
            if (gcmApiKey == null)
            {
                _gcmApiKey = null;
                return;
            }

            if (string.IsNullOrEmpty(gcmApiKey))
            {
                throw new ArgumentException(@"The GCM API Key should be a non-empty string or null.");
            }

            _gcmApiKey = gcmApiKey;
        }


        /// <summary>
        ///     To get a request without sending a push notification call this method.
        ///     This method will throw an ArgumentException if there is an issue with the input.
        /// </summary>
        /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
        /// <param name="payload">The payload you wish to send to the user</param>
        /// <returns>A HttpRequestMessage object that can be sent.</returns>
        protected HttpRequestMessage GenerateRequestDetails(PushSubscription subscription, string payload)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (!Uri.IsWellFormedUriString(subscription.Endpoint, UriKind.Absolute))
            {
                throw new ArgumentException(@"You must pass in a subscription with at least a valid endpoint");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Endpoint);
            var currentGcmApiKey = _gcmApiKey;

            request.Headers.Add("TTL", DefaultTtl.ToString());
            var encryptedPayload = Encryptor.Encrypt(subscription.P256DH, subscription.Auth, payload);
            request.Content = new ByteArrayContent(encryptedPayload.Payload);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content.Headers.ContentLength = encryptedPayload.Payload.Length;
            request.Content.Headers.ContentEncoding.Add("aesgcm");
            request.Headers.Add("Encryption", "salt=" + encryptedPayload.Base64EncodeSalt());
            var cryptoKeyHeader = @"dh=" + encryptedPayload.Base64EncodePublicKey();


            var isGcm = subscription.Endpoint.StartsWith(@"https://android.googleapis.com/gcm/send");
            if (isGcm)
            {
                if (!string.IsNullOrEmpty(currentGcmApiKey))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", $"key={currentGcmApiKey}");
                }
            }
            else if (_vapidDetails != null)
            {
                var uri = new Uri(subscription.Endpoint);
                var audience = $@"{uri.Scheme}://{uri.Host}";

                var vapidHeaders = VapidHelper.GetHttpHeaders(audience, _vapidDetails);
                request.Headers.Add(@"Authorization", vapidHeaders.Authorization);
                if (string.IsNullOrEmpty(cryptoKeyHeader))
                {
                    cryptoKeyHeader = vapidHeaders.CryptoKey;
                }
                else
                {
                    cryptoKeyHeader += @";" + vapidHeaders.CryptoKey;
                }
            }

            request.Headers.Add("Crypto-Key", cryptoKeyHeader);
            return request;
        }

        /// <summary>
        ///     To send a push notification call this method with a subscription, optional payload and any options
        ///     Will exception if unsuccessful
        /// </summary>
        /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
        /// <param name="notification">The payload you wish to send to the user</param>
        public async Task NotifyAsync(PushSubscription subscription, PushNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var request = GenerateRequestDetails(subscription, json);
            var response = await _httpClient.SendAsync(request);
            await HandleResponse(response, subscription);
        }

        /// <summary>
        ///     Handle Web Push responses.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="subscription"></param>
        private static async Task HandleResponse(HttpResponseMessage response, PushSubscription subscription)
        {
            // Successful
            if (response.StatusCode == HttpStatusCode.Created ||
                response.StatusCode == HttpStatusCode.Accepted)
            {
                return;
            }

            var msg = await response.Content.ReadAsStringAsync();
            throw new WebPushException($"{response.StatusCode}: {msg}", response.StatusCode, response.Headers, subscription);
        }

        public void Dispose()
        {
            if (_httpClient == null) 
                return;

            _httpClient.Dispose();
            _httpClient = null;
        }
    }
}