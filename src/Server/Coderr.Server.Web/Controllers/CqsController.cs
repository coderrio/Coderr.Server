using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Infrastructure.Messaging;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.Web.Infrastructure.Cqs;
using codeRR.Server.Web.Models;
using DotNetCqs;
using Griffin;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace codeRR.Server.Web.Controllers
{
    [Authorize]
    public class CqsController : ApiController
    {
        private readonly IMessageBus _messageBus;
        private IQueryBus _queryBus;
        private static readonly CqsObjectMapper _cqsObjectMapper = new CqsObjectMapper();
        private static readonly MessagingSerializer _serializer = new MessagingSerializer();
        private readonly ILog _logger = LogManager.GetLogger(typeof(CqsController));
        private static readonly MethodInfo _queryMethod;
        private static readonly MethodInfo _sendMethod;

        static CqsController()
        {
            if (_cqsObjectMapper.IsEmpty)
                _cqsObjectMapper.ScanAssembly(typeof(CreateApplication).Assembly);

            _queryMethod = typeof(IQueryBus)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(x => x.Name == "QueryAsync" && x.GetParameters().Length == 2);

            _sendMethod = typeof(IMessageBus).GetMethod("SendAsync", new Type[] { typeof(ClaimsPrincipal), typeof(object) });
        }

        public CqsController(IMessageBus messageBus, IQueryBus queryBus)
        {
            _messageBus = messageBus;
            _queryBus = queryBus;
        }

        [Route("api/authenticate"), Route("authenticate"), HttpPost]
        public async Task<AuthenticatedUser> Authenticate()
        {
            var q = new GetApplicationList {AccountId = User.GetAccountId()};
            var result = await _queryBus.QueryAsync((ClaimsPrincipal)User, q);

            return new AuthenticatedUser
            {
                AccountId = User.GetAccountId(),
                UserName = User.Identity.Name,
                Applications = result
            };
        }

        [HttpGet]
        [HttpPost]
        [Route("api/cqs")]
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

            object cqsObject, cqsReplyObject = null;
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
                    prop.SetValue(cqsObject, User.GetAccountId());
                prop = cqsObject.GetType().GetProperty("AccountId");
                if ((prop != null) && prop.CanWrite)
                    prop.SetValue(cqsObject, User.GetAccountId());
                prop = cqsObject.GetType().GetProperty("UserId");
                if ((prop != null) && prop.CanWrite)
                    prop.SetValue(cqsObject, User.GetAccountId());
            }

            RestrictOnApplicationId(cqsObject);

            Exception ex = null;
            try
            {
                _logger.Debug("Invoking " + cqsObject.GetType().Name + " " + json);
                if (IsQuery(cqsObject))
                {
                    cqsReplyObject = await InvokeQuery(cqsObject);
                }
                else
                {
                    await InvokeMessage(cqsObject);
                }

                if (cqsReplyObject != null)
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
                response.StatusCode = (HttpStatusCode)((HttpException)ex).GetHttpCode();
                response.ReasonPhrase = FirstLine(ex.Message);
                return response;
            }
            if (ex is InvalidCredentialException)
            {
                _logger.Error("Auth error for " + json, ex);
                var authEx = (InvalidCredentialException)ex;
                var response = Request.CreateResponse(HttpStatusCode.Forbidden);
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
            if (cqsReplyObject != null)
            {
                reply.Headers.Add("X-Cqs-Object-Type", cqsReplyObject.GetType().GetSimpleAssemblyQualifiedName());
                reply.Headers.Add("X-Cqs-Name", cqsReplyObject.GetType().Name);
                if (cqsReplyObject is Exception)
                    reply.StatusCode = (HttpStatusCode)500;

                json = _serializer.Serialize(cqsReplyObject, out var cnt);
                _logger.Debug("Reply to " + cqsObject.GetType().Name + ": " + json);
                reply.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
            {
                _logger.Debug("Reply to " + cqsObject.GetType().Name + ": [empty response]");
                reply.StatusCode = HttpStatusCode.NoContent;
            }

            return reply;
        }

        private async Task<object> InvokeQuery(object dto)
        {
            var type = dto.GetType();
            var replyType = type.BaseType.GetGenericArguments()[0];
            var method = _queryMethod.MakeGenericMethod(replyType);
            try
            {
                var result = method.Invoke(_queryBus, new[] { User, dto });
                var task = (Task)result;
                await task;
                return ((dynamic)task).Result;
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }

        private async Task InvokeMessage(object dto)
        {
            var type = dto.GetType();
            try
            {
                var task = (Task)_sendMethod.Invoke(_messageBus, new[] { (ClaimsPrincipal)User, dto });
                await task;
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }

        public static bool IsQuery(object cqsObject)
        {
            var baseType = cqsObject.GetType().BaseType;
            while (baseType != null)
            {
                if (baseType.FullName.StartsWith("DotNetCqs.Query"))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        private string FirstLine(string msg)
        {
            var pos = msg.IndexOfAny(new[] { '\r', '\n' });
            return pos == -1 ? msg : msg.Substring(0, pos);
        }

        private async Task HandleSecurityPrincipalUpdates()
        {
            var gotUpdate = ClaimsPrincipal.Current.Identities.First().TryRemoveClaim(CoderrClaims.UpdateIdentity);

            //to be sure that there are no other points in the flow that added the same claim
            while (ClaimsPrincipal.Current.Identities.First().TryRemoveClaim(CoderrClaims.UpdateIdentity))
            {
            }

            if (gotUpdate)
            {
                var usr = (ClaimsPrincipal)User;
                var c = ClaimsPrincipal.Current;
                var t = usr.GetHashCode() == c.GetHashCode();

                var context = Request.GetOwinContext();
                var authenticationContext =
                    await context.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);

                if (authenticationContext != null)
                {
                    context.Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(
                        (ClaimsPrincipal)User,
                        authenticationContext.Properties);
                }
            }
        }

        private void RestrictOnApplicationId(object cqsObject)
        {
            if (ClaimsPrincipal.Current.Identity.AuthenticationType != "ApiKey")
                return;
            if (ClaimsPrincipal.Current.IsInRole(CoderrClaims.RoleSysAdmin))
                return;

            var prop = cqsObject.GetType().GetProperty("ApplicationId");
            if ((prop == null) || !prop.CanRead)
                return;

            var value = (int)prop.GetValue(cqsObject);
            if (!ClaimsPrincipal.Current.IsApplicationMember(value))
            {
                _logger.Warn("Tried to access an application without privileges. accountId: " + User.Identity.Name + ", appId: " + value);
                throw new HttpException(403, "The given application key is not allowed for application " + value);
            }

        }
    }
}