using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;
using Griffin.Cqs.Http;
using codeRR.Infrastructure.Security;

namespace codeRR.Web.Infrastructure.Cqs
{
    public class CqsClientFactory
    {
        public static CqsHttpClient Create()
        {
            var url = ConfigurationManager.AppSettings["CqsHttpServer"] ?? "http://127.0.0.1:8873";
            var cqsClient = new CqsHttpClient(url)
            {
                CqsSerializer = new CqsJsonNetSerializer(),
                RequestDecorator = request =>
                {
                    if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/Account/Activate"))
                        Debugger.Break();

                    if (ClaimsPrincipal.Current.Identity.IsAuthenticated)
                    {
                        request.Headers.Add("X-UserName", ClaimsPrincipal.Current.Identity.Name);
                        request.Headers.Add("X-AccountId", ClaimsPrincipal.Current.GetAccountId().ToString());
                    }
                }
            };
            return cqsClient;
        }
    }
}