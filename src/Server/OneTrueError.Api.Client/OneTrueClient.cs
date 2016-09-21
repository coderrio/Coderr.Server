using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetCqs;
using Newtonsoft.Json;
using OneTrueError.Api.Client.Json;
using OneTrueError.Api.Client.Tests;

namespace OneTrueError.Api.Client
{
    /// <summary>
    /// Client for the OneTrueError server API
    /// </summary>
    public class OneTrueClient : IQueryBus, ICommandBus, IEventBus
    {
        private CookieContainer _cookies;
        private Encoding _basicAuthEncoding = Encoding.GetEncoding("ISO-8859-1");

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented
        };

        private Uri _uri;


        public OneTrueClient()
        {
            _jsonSerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
        }

        public NetworkCredential Credentials { get; set; }

        public async Task ExecuteAsync<T>(T command) where T : Command
        {
            var response = await RequestAsync("POST", "command", command);
            response.Close();
        }

        public async Task PublishAsync<TApplicationEvent>(TApplicationEvent e)
            where TApplicationEvent : ApplicationEvent
        {
            var response = await RequestAsync("POST", "event", e);
            response.Close();
        }

        public async Task<TResult> QueryAsync<TResult>(Query<TResult> query)
        {
            //TODO: Unwrap the cqs object to query parameters instead
            //to allow caching in the server
            var response = await RequestAsync("POST", "query", query);
            return await DeserializeResponse<TResult>(response);
        }

        public void Open(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _uri = uri;
            _cookies = new CookieContainer();
        }

        private async Task<TResult> DeserializeResponse<TResult>(HttpWebResponse response)
        {
            var responseStream = response.GetResponseStream();
            var jsonBuf = new byte[responseStream.Length];
            await responseStream.ReadAsync(jsonBuf, 0, jsonBuf.Length);
            var jsonStr = Encoding.UTF8.GetString(jsonBuf);
            var responseObj = JsonConvert.DeserializeObject(jsonStr, typeof(TResult), _jsonSerializerSettings);
            return (TResult)responseObj;
        }

        private async Task<HttpWebResponse> RequestAsync(string httpMethod, string cqsType, object cqsObject)
        {
            if (_cookies.Count == 0)
            {
                var sb = new StringBuilder();
                sb.AppendUrlEncoded("username", Credentials.UserName);
                sb.AppendUrlEncoded("password", Credentials.Password);
                var data = Encoding.UTF8.GetBytes(sb.ToString());

                var authRequest = WebRequest.CreateHttp(_uri + "/account/login");
                authRequest.Method = "POST";
                authRequest.CookieContainer = _cookies;
                authRequest.ContentType = "application/x-www-form-urlencoded";
                authRequest.ContentLength = data.Length;
                var authReqStream = await authRequest.GetRequestStreamAsync();
                await authReqStream.WriteAsync(data, 0, data.Length);
                var resp = authRequest.GetResponse();

            }
            string authInfo = Credentials.UserName + ":" + Credentials.Password;
            authInfo = Convert.ToBase64String(_basicAuthEncoding.GetBytes(authInfo));

            var request = WebRequest.CreateHttp(_uri + "api/cqs");
            request.Method = httpMethod;
            request.Headers.Add("Authorization", "Basic " + authInfo);
            request.Headers.Add("X-Cqs-Name", cqsObject.GetType().Name);
            //request.PreAuthenticate = true;
            request.CookieContainer = _cookies;

            var stream = await request.GetRequestStreamAsync();
            var json = JsonConvert.SerializeObject(cqsObject, _jsonSerializerSettings);
            var buffer = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            return (HttpWebResponse)await request.GetResponseAsync();
        }
    }
}