using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Api.Client.Json;
using DotNetCqs;
using Newtonsoft.Json;

namespace Coderr.Server.Api.Client
{
    /// <summary>
    ///     Client for the Coderr server API
    /// </summary>
    public class ServerApiClient : IMessageBus, IQueryBus
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IncludeNonPublicMembersContractResolver()
        };

        private string _apiKey;
        private string _sharedSecret;
        private Uri _uri;


        /// <summary>
        /// Send a query to the server.
        /// </summary>
        /// <param name="principal">Principal for the user making the request</param>
        /// <param name="message">Message being sent</param>
        /// <returns>task</returns>
        async Task IMessageBus.SendAsync(ClaimsPrincipal principal, object message)
        {
            await RequestAsync(HttpMethod.Post, "send", message);
        }

        async Task IMessageBus.SendAsync(ClaimsPrincipal principal, Message message)
        {
            await RequestAsync(HttpMethod.Post, "send", message.Body);
        }

        async Task IMessageBus.SendAsync(Message message)
        {
            await RequestAsync(HttpMethod.Post, "send", message.Body);
        }

        /// <summary>
        ///     Send a command or event
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>task</returns>
        public async Task SendAsync(object message)
        {
            await RequestAsync(HttpMethod.Post, "send", message);
        }

        async Task<TResult> IQueryBus.QueryAsync<TResult>(ClaimsPrincipal user, Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync(HttpMethod.Post, "query", query);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return default(TResult);
            return await DeserializeResponse<TResult>(response.Content);
        }

        /// <summary>
        ///     Make a query
        /// </summary>
        /// <typeparam name="TResult">Type of result that the query returns</typeparam>
        /// <param name="query">query to invoke</param>
        /// <returns>task</returns>
        public async Task<TResult> QueryAsync<TResult>(Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync(HttpMethod.Post, "query", query);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return default(TResult);
            response.EnsureSuccessStatusCode();
            return await DeserializeResponse<TResult>(response.Content);
        }


        /// <summary>
        ///     Open a channel
        /// </summary>
        /// <param name="uri">Root URL to the Coderr web</param>
        /// <param name="apiKey">API key from the administration area in Coderr web</param>
        /// <param name="sharedSecret">Shared secret from the administration area in Coderr web</param>
        public void Open(Uri uri, string apiKey, string sharedSecret)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _sharedSecret = sharedSecret ?? throw new ArgumentNullException(nameof(sharedSecret));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }


        private async Task<TResult> DeserializeResponse<TResult>(HttpContent content)
        {
            var jsonStr = await content.ReadAsStringAsync();
            try
            {
                var responseObj = JsonConvert.DeserializeObject(jsonStr, typeof(TResult), _jsonSerializerSettings);
                return (TResult) responseObj;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize " + jsonStr, ex);
            }
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod httpMethod, string cqsType, object cqsObject)
        {
            var request = new HttpRequestMessage(httpMethod, $"{_uri}api/cqs");
            request.Headers.Add("X-Api-Key", _apiKey);
            request.Headers.Add("X-Cqs-Name", cqsObject.GetType().Name);

            var json = JsonConvert.SerializeObject(cqsObject, _jsonSerializerSettings);
            var buffer = Encoding.UTF8.GetBytes(json);
            var hamc = new HMACSHA256(Encoding.UTF8.GetBytes(_sharedSecret.ToLower()));
            var hash = hamc.ComputeHash(buffer);
            var signature = Convert.ToBase64String(hash);
            request.Headers.Add("Authorization", "ApiKey " + _apiKey + " " + signature);
            request.Headers.Add("X-Api-Signature", signature);

            request.Content = new ByteArrayContent(buffer);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode >= 500)
                throw new HttpRequestException(response.ReasonPhrase);
            return response;
        }
    }
}