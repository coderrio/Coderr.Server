using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using codeRR.Client;
using codeRR.Server.Infrastructure;
using codeRR.Server.SqlServer;
using codeRR.Server.Web.Infrastructure.Logging;
using log4net;
using log4net.Config;
using codeRR.Client.Contracts;

namespace codeRR.Server.Web
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILog _logger;

        static WebApiApplication()
        {
            var path2 = AppDomain.CurrentDomain.BaseDirectory;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(path2, "log4net.config")));
            _logger = LogManager.GetLogger(typeof(WebApiApplication));
            _logger.Info("Loaded");

            Griffin.Logging.LogManager.Provider = new LogManagerAdapter();
        }

        protected void Application_Start()
        {
            _logger.Info("Started");

            SetupTools.DbTools = new SqlServerTools();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
        }

        private void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            var data = "";
            if ((Request.InputStream != null) && (Request.InputStream.Length > 0))
            {
                var reader = new StreamReader(Request.InputStream);
                data = reader.ReadToEnd();
            }
            _logger.Error("Request + " + Request.Url + ", data" + data, exception);

            var properties = new Dictionary<string, string>
            {
                {"Url", Request.Url.ToString()},
                {"HttpMethod", Request.HttpMethod}
            };
            if (Request.UrlReferrer != null)
                properties.Add("Referrer", Request.UrlReferrer.ToString());
            if (data.Length < 30000)
                properties.Add("Body", data);
            properties.Add("ErrTags", "unhandled-exception");
            var collection = new ContextCollectionDTO("Request", properties);
            Err.Report(exception, collection);
        }
        
    }
}