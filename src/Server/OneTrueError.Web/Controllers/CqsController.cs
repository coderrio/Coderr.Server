using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DotNetCqs;
using Griffin;
using Griffin.Cqs;
using Griffin.Cqs.Authorization;
using Griffin.Cqs.Net;
using OneTrueError.Api.Core.Applications.Commands;
using OneTrueError.Web.Infrastructure.Cqs;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Controllers
{
    [System.Web.Http.Authorize]
    public class CqsController : ApiController
    {
        private static readonly CqsObjectMapper _cqsObjectMapper = new CqsObjectMapper();
        private static readonly CqsJsonNetSerializer _serializer = new CqsJsonNetSerializer();
        private readonly CqsMessageProcessor _cqsProcessor;

        static CqsController()
        {
            if (_cqsObjectMapper.IsEmpty)
                _cqsObjectMapper.ScanAssembly(typeof (CreateApplication).Assembly);
        }

        public CqsController(IQueryBus queryBus, IRequestReplyBus requestReplyBus, ICommandBus commandBus,
            IEventBus eventBus)
        {
            _cqsProcessor = new CqsMessageProcessor
            {
                CommandBus = commandBus,
                RequestReplyBus = requestReplyBus,
                EventBus = eventBus,
                QueryBus = queryBus
            };
        }


        [Route("api/cqs"), HttpGet, HttpPost]
        public async Task<HttpResponseMessage> Cqs()
        {
            var dotNetType = Request.Headers.Contains("X-Cqs-Object-Type")
                ? Request.Headers.GetValues("X-Cqs-Object-Type").FirstOrDefault()
                : null;
            var cqsName = Request.Headers.Contains("X-Cqs-Name")
                ? Request.Headers.GetValues("X-Cqs-Name").FirstOrDefault()
                : null;

            var json = await Request.Content.ReadAsStringAsync();

            object cqsObject;
            if (!string.IsNullOrEmpty(dotNetType))
            {
                cqsObject = _cqsObjectMapper.Deserialize(dotNetType, json);
                if (cqsObject == null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = "Unknown type: " + dotNetType;
                    return response;
                }
            }
            else if (!string.IsNullOrEmpty(cqsName))
            {
                cqsObject = _cqsObjectMapper.Deserialize(cqsName, json);
                if (cqsObject == null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = "Unknown type: " + cqsName;
                    return response;
                }
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase =
                    "Expected a class name in the header 'X-Cqs-Name' or a .NET type name in the header 'X-Cqs-Object-Type'.";
                return response;
            }

            var prop = cqsObject.GetType().GetProperty("UserId");
            if (prop != null && prop.CanWrite)
                prop.SetValue(cqsObject, SessionUser.Current.AccountId);

            prop = cqsObject.GetType().GetProperty("AccountId");
            if (prop != null && prop.CanWrite)
                prop.SetValue(cqsObject, SessionUser.Current.AccountId);

            ClientResponse cqsReplyObject = null;
            Exception ex = null;
            try
            {
                cqsReplyObject = await _cqsProcessor.ProcessAsync(cqsObject);
            }
            catch (AggregateException e1)
            {
                ex = e1.InnerException;
            }

            if (ex is HttpException)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = (HttpStatusCode) ((HttpException) ex).GetHttpCode();
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }
            if (ex is AuthorizationException)
            {
                var authEx = (AuthorizationException) ex;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }
            if (ex != null)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }

            var reply = Request.CreateResponse(HttpStatusCode.OK);

            // for instance commands do not have a return value.
            if (cqsReplyObject.Body != null)
            {
                reply.Headers.Add("X-Cqs-Object-Type", cqsReplyObject.Body.GetType().GetSimpleAssemblyQualifiedName());
                reply.Headers.Add("X-Cqs-Name", cqsReplyObject.Body.GetType().Name);
                if (cqsReplyObject.Body is Exception)
                    reply.StatusCode = (HttpStatusCode) 500;

                string cnt;
                json = _serializer.Serialize(cqsReplyObject.Body, out cnt);
                reply.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
                reply.StatusCode = HttpStatusCode.NoContent;

            return reply;
        }

        private string FirstLine(string msg)
        {
            var pos = msg.IndexOfAny(new[] {'\r', '\n'});
            if (pos == -1)
                return msg;

            return msg.Substring(0, pos);
        }
    }
}