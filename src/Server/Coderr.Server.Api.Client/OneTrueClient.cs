using System;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using codeRR.Server.Api.Client.Json;
using DotNetCqs;
using Newtonsoft.Json;

namespace codeRR.Server.Api.Client
{
    /// <summary>
    ///     Client for the codeRR server API
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

        async Task<TResult> IQueryBus.QueryAsync<TResult>(ClaimsPrincipal user, Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync("POST", "query", query);
            return await DeserializeResponse<TResult>(response);

        }

        public async Task<TResult> QueryAsync<TResult>(Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync("POST", "query", query);
            return await DeserializeResponse<TResult>(response);
        }

        
        async Task IMessageBus.SendAsync(ClaimsPrincipal principal, object message)
        {
            await RequestAsync("POST", "send", message);
        }

        async Task IMessageBus.SendAsync(ClaimsPrincipal principal, Message message)
        {
            await RequestAsync("POST", "send", message.Body);
        }

        async Task IMessageBus.SendAsync(Message message)
        {
            await RequestAsync("POST", "send", message.Body);
        }

        public async Task SendAsync(object message)
        {
            await RequestAsync("POST", "send", message);
        }


        /// <summary>
        ///     Open a channel
        /// </summary>
        /// <param name="uri">Root URL to the codeRR web</param>
        /// <param name="apiKey">API key from the administration area in codeRR web</param>
        /// <param name="sharedSecret">Shared secret from the administration area in codeRR web</param>
        public void Open(Uri uri, string apiKey, string sharedSecret)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _sharedSecret = sharedSecret ?? throw new ArgumentNullException(nameof(sharedSecret));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }
        

        private async Task<TResult> DeserializeResponse<TResult>(HttpWebResponse response)
        {
            var responseStream = response.GetResponseStream();
            var jsonBuf = new byte[response.ContentLength];
            await responseStream.ReadAsync(jsonBuf, 0, jsonBuf.Length);
            var jsonStr = Encoding.UTF8.GetString(jsonBuf);
            var responseObj = JsonConvert.DeserializeObject(jsonStr, typeof(TResult), _jsonSerializerSettings);
            return (TResult) responseObj;
        }

        private async Task<HttpWebResponse> RequestAsync(string httpMethod, string cqsType, object cqsObject)
        {
            var request = WebRequest.CreateHttp(_uri + "api/cqs");
            request.Method = httpMethod;
            request.Headers.Add("X-Api-Key", _apiKey);
            request.Headers.Add("X-Cqs-Name", cqsObject.GetType().Name);

            var stream = await request.GetRequestStreamAsync();
            var json = JsonConvert.SerializeObject(cqsObject, _jsonSerializerSettings);
            var buffer = Encoding.UTF8.GetBytes(json);

            var hamc = new HMACSHA256(Encoding.UTF8.GetBytes(_sharedSecret.ToLower()));
            var hash = hamc.ComputeHash(buffer);
            var signature = Convert.ToBase64String(hash);

            await stream.WriteAsync(buffer, 0, buffer.Length);

            request.Headers.Add("X-Api-Signature", signature);

            return (HttpWebResponse) await request.GetResponseAsync();
        }
    }
}