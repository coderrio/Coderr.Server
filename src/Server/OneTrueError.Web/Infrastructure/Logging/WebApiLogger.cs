using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using log4net;

namespace OneTrueError.Web.Infrastructure.Logging
{
    class WebApiLogger : ExceptionLogger
    {
        private ILog _logger = LogManager.GetLogger(typeof (WebApiLogger));

        public override void Log(ExceptionLoggerContext context)
        {
            var data = "";
            if (context.Request.Content != null)
            {
                data = context.Request.Content.ReadAsStringAsync().Result;
            }

            _logger.Error("Request + " + context.Request.RequestUri + ", data" + data, context.Exception);
            _logger.Error(context.Exception);
            base.Log(context);
        }

    }
}