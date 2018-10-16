using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coderr.Server.Web.Infrastructure
{
    public class TransactionalAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null && filterContext.ModelState.IsValid && filterContext.HttpContext.Request.Method == "POST")
            {
                var uow = (IAdoNetUnitOfWork) filterContext.HttpContext.RequestServices.GetService(typeof(IAdoNetUnitOfWork));
                uow.SaveChanges();
            }
                
            base.OnActionExecuted(filterContext);
        }
    }
}
