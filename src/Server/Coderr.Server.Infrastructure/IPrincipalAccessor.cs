//using System.Security.Claims;

//namespace Coderr.Server.Infrastructure
//{
//    public class PrincipalWrapper
//    {
//        private ClaimsPrincipal _principal;

//        public ClaimsPrincipal Principal
//        {
//            get { return _principal; }
//            set
//            {
//                if (value == null || !value.Identity.IsAuthenticated)
//                    return;

//                _principal = value;
//            }
//        }
//    }
//}