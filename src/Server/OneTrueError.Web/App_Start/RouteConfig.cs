using System.Web.Mvc;
using System.Web.Routing;

namespace OneTrueError.Web
{
    public class RouteConfig
    {
        public static void RegisterInstallationRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "ToInstall", id = UrlParameter.Optional},
                new[] {"OneTrueError.Web.Controllers"}
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
                new[] {"OneTrueError.Web.Controllers"}
                );
        }
    }
}