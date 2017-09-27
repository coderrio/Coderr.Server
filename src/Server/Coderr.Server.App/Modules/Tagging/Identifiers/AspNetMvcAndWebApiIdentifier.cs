using System;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Identifiers
{
    /// <summary>
    ///     Identifies ASP.NET MVC and WebApi including their versions.
    /// </summary>
    [Component]
    public class AspNetMvcAndWebApiIdentifier : ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        public void Identify(TagIdentifierContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            var orderNumber = context.AddIfFound("System.Web.Mvc", "asp.net-mvc");
            if (orderNumber != -1)
            {
                var propertyValue = context.GetPropertyValue("Assemblies", "System.Web.Mvc");
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    var version = propertyValue.Substring(0, 1);
                    if (version != "0" && version != "1")
                        context.AddTag("asp.net-mvc-" + version, orderNumber);
                }

                context.AddTag("asp.net", 99);
            }

            orderNumber = context.AddIfFound("System.Web.Http.WebHost", "asp.net-web-api");
            if (orderNumber != -1)
            {
                var propertyValue = context.GetPropertyValue("Assemblies", "System.Web.Http.WebHost");
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    var version = propertyValue.Substring(0, 1);
                    context.AddTag("asp.net-web-api-" + version, orderNumber);
                }

                context.AddTag("asp.net", 99);
            }
        }
    }
}