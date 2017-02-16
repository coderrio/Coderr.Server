using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using Griffin.Signals;
using log4net;
using log4net.Config;
using OneTrueError.Client;
using OneTrueError.Client.Contracts;
using OneTrueError.Infrastructure;
using OneTrueError.SqlServer;
using OneTrueError.Web.Infrastructure.Logging;

namespace OneTrueError.Web
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILog _logger;
        public static bool ReportToOneTrueError;

        static WebApiApplication()
        {
            Signal.SignalRaised += OnSignalRaised;
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

            if (!ReportToOneTrueError)
                return;

            var properties = new Dictionary<string, string>
            {
                {"Url", Request.Url.ToString()},
                {"HttpMethod", Request.HttpMethod}
            };
            if (Request.UrlReferrer != null)
                properties.Add("Referer", Request.UrlReferrer.ToString());
            if (data.Length < 30000)
                properties.Add("Body", data);
            properties.Add("OneTrueTags", "unhandled-exception");
            var collection = new ContextCollectionDTO("Request", properties);
            OneTrue.Report(exception, collection);
        }

        private static void OnSignalRaised(object sender, SignalRaisedEventArgs e)
        {
            _logger.Debug(e.SignalName + ": " + e.Reason + ", " + e.Exception);
        }
    }
}