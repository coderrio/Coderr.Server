using System.Web.Http;
using OneTrueError.Web.Infrastructure;
using OneTrueError.Web.Infrastructure.Auth;

namespace OneTrueError.Web
{
    public static class WebApiConfig
    {
        public static string UrlPrefix
        {
            get { return "api"; }
        }

        public static string UrlPrefixRelative
        {
            get { return "~/api"; }
        }

        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new ApiKeyAuthenticator());
            config.Filters.Add(new WebApiAuthenticationFilter());
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );
        }
    }
}