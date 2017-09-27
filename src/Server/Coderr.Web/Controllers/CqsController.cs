using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DotNetCqs;
using Griffin;
using Griffin.Cqs;
using Griffin.Cqs.Authorization;
using Griffin.Cqs.Net;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using codeRR.Api.Core.Applications.Commands;
using codeRR.Infrastructure.Security;
using codeRR.Web.Infrastructure.Cqs;

namespace codeRR.Web.Controllers
{
    [System.Web.Http.Authorize]
    public class CqsController : ApiController
    {
        private static readonly CqsObjectMapper _cqsObjectMapper = new CqsObjectMapper();
        private static readonly CqsJsonNetSerializer _serializer = new CqsJsonNetSerializer();
        private readonly CqsMessageProcessor _cqsProcessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CqsController));

        static CqsController()
        {
            if (_cqsObjectMapper.IsEmpty)
                _cqsObjectMapper.ScanAssembly(typeof(CreateApplication).Assembly);
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


        [Route("api/cqs")]
        [HttpGet]
        [HttpPost]
        [Route("cqs")]
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
                    _logger.Error($"Could not deserialize[{dotNetType}]: {json}");
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
                    _logger.Error($"Could not deserialize[{cqsName}]: {json}");
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

            if (User.Identity.AuthenticationType != "ApiKey")
            {
                var prop = cqsObject.GetType().GetProperty("CreatedById");
                if ((prop != null) && prop.CanWrite)
                    prop.SetValue(cqsObject, ClaimsPrincipal.Current.GetAccountId());
                prop = cqsObject.GetType().GetProperty("AccountId");
                if ((prop != null) && prop.CanWrite)
                    prop.SetValue(cqsObject, ClaimsPrincipal.Current.GetAccountId());
                prop = cqsObject.GetType().GetProperty("UserId");
                if ((prop != null) && prop.CanWrite)
                    prop.SetValue(cqsObject, ClaimsPrincipal.Current.GetAccountId());
            }

            RestrictOnApplicationId(cqsObject);

            ClientResponse cqsReplyObject = null;
            Exception ex = null;
            try
            {
                _logger.Debug("Invoking " + cqsObject.GetType().Name + " " + json);
                cqsReplyObject = await _cqsProcessor.ProcessAsync(cqsObject);
                RestrictOnApplicationId(cqsReplyObject);
                await HandleSecurityPrincipalUpdates();
            }
            catch (AggregateException e1)
            {
                _logger.Error("Failed to process '" + json + "'.", e1);
                ex = e1.InnerException;
            }
            catch (Exception e1)
            {
                _logger.Error("Failed to process2 '" + json + "'.", e1);
                ex = e1;
            }

            if (ex is HttpException)
            {
                _logger.Error("HTTP error for " + json, ex);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = (HttpStatusCode) ((HttpException) ex).GetHttpCode();
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }
            if (ex is AuthorizationException)
            {
                _logger.Error("Auth error for " + json, ex);
                var authEx = (AuthorizationException) ex;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }
            if (ex != null)
            {
                _logger.Error("Failed to process result for " + json, ex);
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
                _logger.Debug("Reply to " + cqsObject.GetType().Name + ": " + json);
                reply.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
            {
                _logger.Debug("Reply to " + cqsObject.GetType().Name + ": [emptry response]");
                reply.StatusCode = HttpStatusCode.NoContent;
            }

            return reply;
        }

        private string FirstLine(string msg)
        {
            var pos = msg.IndexOfAny(new[] {'\r', '\n'});
            return pos == -1 ? msg : msg.Substring(0, pos);
        }

        private async Task HandleSecurityPrincipalUpdates()
        {
            var gotUpdate = ClaimsPrincipal.Current.Identities.First().TryRemoveClaim(OneTrueClaims.UpdateIdentity);

            //to be sure that there are no other points in the flow that added the same claim
            while (ClaimsPrincipal.Current.Identities.First().TryRemoveClaim(OneTrueClaims.UpdateIdentity))
            {
            }

            if (gotUpdate)
            {
                var usr = (ClaimsPrincipal) User;
                var c = ClaimsPrincipal.Current;
                var t = usr.GetHashCode() == c.GetHashCode();

                var context = Request.GetOwinContext();
                var authenticationContext =
                    await context.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);

                if (authenticationContext != null)
                {
                    context.Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(
                        (ClaimsPrincipal) User,
                        authenticationContext.Properties);
                }
            }
        }

        private void RestrictOnApplicationId(object cqsObject)
        {
            if (ClaimsPrincipal.Current.Identity.AuthenticationType != "ApiKey")
                return;
            if (ClaimsPrincipal.Current.IsInRole(OneTrueClaims.RoleSysAdmin))
                return;

            var prop = cqsObject.GetType().GetProperty("ApplicationId");
            if ((prop == null) || !prop.CanRead)
                return;

            var value = (int) prop.GetValue(cqsObject);
            if (!ClaimsPrincipal.Current.IsApplicationMember(value))
            {
                _logger.Warn("Tried to access an application without privileges. accountId: " + User.Identity.Name + ", appId: " + value);
                throw new HttpException(403, "The given application key is not allowed for application " + value);
            }
                
        }
    }
}