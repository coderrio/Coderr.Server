using System.Web.Mvc;
using System.Web.Routing;

namespace codeRR.Server.Web
{
    public class RouteConfig
    {
        public static void RegisterInstallationRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Boot", action = "ToInstall", id = UrlParameter.Optional},
                new[] {"codeRR.Server.Web.Controllers"}
                );
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.RouteExistingFiles = true;
            //routes.MapRoute("InstallationOff",
            //    "installation/{*catchAll}",
            //    new { controller = "Home", action = "NoInstallation" });
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] { "codeRR.Server.Web.Controllers" }
                );
        }
    }
}