using System.Configuration;
using System.Diagnostics;
using System.Web;
using Griffin.Cqs.Http;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Infrastructure.Cqs
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

                    if (SessionUser.IsAuthenticated)
                    {
                        request.Headers.Add("X-UserName", SessionUser.Current.UserName);
                        request.Headers.Add("X-AccountId", SessionUser.Current.AccountId.ToString());
                    }
                }
            };
            return cqsClient;
        }
    }
}