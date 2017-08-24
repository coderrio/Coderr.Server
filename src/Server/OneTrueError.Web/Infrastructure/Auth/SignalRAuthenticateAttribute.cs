//using System;
//using System.Security.Claims;
//using System.Security.Principal;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;

//namespace Monolith.Web.Infrastructure
//{
//    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
//    public class SignalRAuthenticateAttribute : AuthorizeAttribute
//    {
//        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
//        {
//            return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
//        }

//        protected override bool UserAuthorized(IPrincipal user)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }


//            var principal = user as ClaimsPrincipal;

//            if (principal != null)
//            {
//                var authenticated = principal.FindFirst(ClaimTypes.Authentication);
//                if (authenticated != null && authenticated.Value == "true")
//                {
//                    return true;
//                }
//                return false;
//            }
//            return false;
//        }
//    }
//}

