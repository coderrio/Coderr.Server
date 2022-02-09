using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Coderr.Client;
using Coderr.Server.SqlServer.ReportAnalyzer;
using Coderr.Server.Web.Infrastructure.Logging;
using Griffin.Data.Mapper;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Coderr.Server.Web
{
    public class Program
    {
        private static ILog _logger;
        private static string _environmentName;

        public static void Main(string[] args)
        {
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() ?? "Production";
            if (Debugger.IsAttached)
            {
                _environmentName = "Development";
            }
#if PREMISE && DEBUG
            _environmentName = "OnPremiseDev";
#endif

            Err.Configuration.ReportSlowRequests(TimeSpan.FromSeconds(1));
            Err.Configuration.AttachUserPrincipalToken();
            //Err.Configuration.ReportSlowExecutes(TimeSpan.FromSeconds(1));
            //Err.Configuration.ReportSlowQueries(TimeSpan.FromSeconds(1));

            ConfigureLog4Net();
            DotNetCqs.LogConfiguration.LogFactory = new MyLogFactoryAdapter();

            var provider = new AssemblyScanningMappingProvider();
            provider.Scan(typeof(AnalyticsRepository).Assembly);
            EntityMappingProvider.Provider = provider;

            var logPath = @"E:\coderr_logs";
            string logName = null;
            if (Directory.Exists(logPath))
                logName = Path.Combine(logPath, $"Coderr_{DateTime.UtcNow:yyyy-MM-dd yyyyMMdd_HHmmssfff}.log");

            try
            {
                if (logName != null)
                    File.AppendAllText(logName, $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} Starting\r\n");

                BuildWebHost(args).Run();

                if (logName != null)
                    File.AppendAllText(logName, $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} Stopped successfully\r\n");
            }
            catch (Exception ex)
            {
                if (logName != null)
                    File.AppendAllText(logName, $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} Stopped due to error, error: {ex}\r\n");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseEnvironment(_environmentName)
                .ConfigureLogging((ctx, logging) =>
                {
                    logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Build();

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
