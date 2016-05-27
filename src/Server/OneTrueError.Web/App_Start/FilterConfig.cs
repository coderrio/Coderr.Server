using System.Web;
using System.Web.Mvc;
using OneTrueError.Web.Infrastructure;

namespace OneTrueError.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomMvcAuthorizeFilter());
        }
    }
}
