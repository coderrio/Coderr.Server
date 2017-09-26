using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetCqs;
using Newtonsoft.Json;
using OneTrueError.Api.Client.Json;

namespace OneTrueError.Api.Client
{
    /// <summary>
    ///     Client for the OneTrueError server API
    /// </summary>
    public class OneTrueApiClient : IQueryBus, ICommandBus, IEventBus
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented
        };

        private string _apiKey;

        private string _sharedSecret;
        private Uri _uri;


        /// <summary>
        ///     Creates a new instance of <see cref="OneTrueApiClient" />.
        /// </summary>
        public OneTrueApiClient()
        {
            _jsonSerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
        }

        /// <summary>
        ///     Execute a command
        /// </summary>
        /// <typeparam name="T">type of query (from the <c>OneTrueError.Api</c> class library)</typeparam>
        /// <param name="command">command to execute</param>
        /// <returns>task</returns>
        public async Task ExecuteAsync<T>(T command) where T : Command
        {
            var response = await RequestAsync("POST", "command", command);
            response.Close();
        }

        /// <summary>
        ///     Publish an event
        /// </summary>
        /// <typeparam name="TApplicationEvent">type of event (from the <c>OneTrueError.Api</c> class library)</typeparam>
        /// <param name="e">event to publish</param>
        /// <returns>task</returns>
        public async Task PublishAsync<TApplicationEvent>(TApplicationEvent e)
            where TApplicationEvent : ApplicationEvent
        {
            var response = await RequestAsync("POST", "event", e);
            response.Close();
        }

        /// <summary>
        ///     Make a query
        /// </summary>
        /// <typeparam name="TResult">Result from a query (a class from the <c>OneTrueError.Api</c> library)</typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TResult> QueryAsync<TResult>(Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync("POST", "query", query);
            return await DeserializeResponse<TResult>(response);
        }

        /// <summary>
        ///     Open a channel
        /// </summary>
        /// <param name="uri">Root URL to the OneTrueError web</param>
        /// <param name="apiKey">Api key from the admin area in OneTrueError web</param>
        /// <param name="sharedSecret">Shared secret from the admin area in OneTrueError web</param>
        public void Open(Uri uri, string apiKey, string sharedSecret)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            if (sharedSecret == null) throw new ArgumentNullException(nameof(sharedSecret));
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _apiKey = apiKey;
            _sharedSecret = sharedSecret;
            _uri = uri;
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