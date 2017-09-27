using System;
using System.Configuration;
using System.IO;
using log4net.Config;

namespace codeRR.Server.Web
{
    public class LogConfigurator
    {
        public static void Configure()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var appType = ConfigurationManager.AppSettings["SiteType"];
            XmlConfigurator.Configure(new FileInfo(Path.Combine(path, "log4net." + appType + ".conf")));
        }
    }
}