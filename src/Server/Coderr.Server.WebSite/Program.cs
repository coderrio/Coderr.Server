using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.SqlServer.ReportAnalyzer;
using Coderr.Server.WebSite.Infrastructure.Adapters.Logging;
using Griffin.Data.Mapper;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Coderr.Server.WebSite
{
    public class Program
    {
        private static ILog _logger;
        private static string _environmentName;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() ?? "Production";

            ConfigureLog4Net();
            Err.Configuration.ReportSlowRequests(TimeSpan.FromSeconds(1));
            Err.Configuration.AttachUserPrincipalToken();
            Err.Configuration.CatchLog4NetExceptions();

            DotNetCqs.LogConfiguration.LogFactory = new MicrosoftLogFactoryAdapter();

            var provider = new AssemblyScanningMappingProvider();
            provider.Scan(typeof(AnalyticsRepository).Assembly);
            EntityMappingProvider.Provider = provider;


            CreateHostBuilder(args).Build().Run();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Unhandled ", (Exception)e.ExceptionObject);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment(_environmentName)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseEnvironment(_environmentName);


        private static void ConfigureLog4Net()
        {
            string logPath;
            if (string.IsNullOrEmpty(_environmentName))
            {
                logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            }
            else
            {
                logPath = $"log4net.{_environmentName}.config";
                if (!File.Exists(logPath))
                {
                    logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log4net.{_environmentName}.config");
                    if (!File.Exists(logPath))
                        logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                }
            }


            var repos = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(repos, new FileInfo(logPath));

            _logger = LogManager.GetLogger(typeof(Program));
            _logger.Info($"Started {_environmentName} from path {logPath}.");

            
        }
    }
}
