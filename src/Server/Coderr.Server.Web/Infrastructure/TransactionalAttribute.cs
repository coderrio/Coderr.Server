using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coderr.Server.Web.Infrastructure
{
    public class TransactionalAttribute2 : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var isMethodTransactional = true;/*filterContext.HttpContext.Request.Method == "POST" ||
                                        filterContext.ActionDescriptor.FilterDescriptors.Any(x =>
                                            x.Filter.GetType() == typeof(TransactionalAttribute));*/

            if (filterContext.Exception == null && filterContext.ModelState.IsValid && isMethodTransactional)
            {
                var uow = (IAdoNetUnitOfWork) filterContext.HttpContext.RequestServices.GetService(typeof(IAdoNetUnitOfWork));
                uow.SaveChanges();
            }
                
            base.OnActionExecuted(filterContext);
        }
    }
}
