using System.Web.Mvc;

namespace OneTrueError.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "admin_default",
                "admin/{controller}/{action}/{id}",
                new {action = "Index", controller = "Home", id = UrlParameter.Optional},
                new[] {GetType().Namespace + ".Controllers"}
                );
        }
    }
}