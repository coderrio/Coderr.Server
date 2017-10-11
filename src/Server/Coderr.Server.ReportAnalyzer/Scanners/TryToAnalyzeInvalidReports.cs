////TODO: Create MSMQ analyzer
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.Common;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using Griffin.ApplicationServices;
//using Griffin.Container;
//using Griffin.Data;
//using log4net;
//using codeRR.MicroService.Core.Authentication;
//using codeRR.ReportAnalyzer.App.Services;

//namespace codeRR.ReportAnalyzer.App.Scanners
//{
//    [Component(Lifetime = Lifetime.Singleton)]
//    public class TryToAnalyzeInvalidReports : ApplicationServiceTimer
//    {
//        private readonly ReportDtoConverter _reportDtoConverter = new ReportDtoConverter();
//        private readonly IScopedTaskInvoker _scopedTaskInvoker;
//        private List<CustomerApp> _applications = new List<CustomerApp>();
//        private ILog _logger = LogManager.GetLogger(typeof (TryToAnalyzeInvalidReports));

//        public TryToAnalyzeInvalidReports(IScopedTaskInvoker scopedTaskInvoker)
//        {
//            _scopedTaskInvoker = scopedTaskInvoker;
//        }

//        protected string BasePath
//        {
//            get
//            {
//                var path = ConfigurationManager.AppSettings["ReportStoragePath"];
//                return !string.IsNullOrEmpty(path)
//                    ? Path.Combine(path, "Failed")
//                    : @"D:\Logs\InvalidReports\";
//            }
//        }

//        private static bool _noAppsWarningIsMade = false;

//        protected override void Execute()
//        {
//            var reader = new FileReader();
//            Dictionary<string, object> headers;
//            string json;
//            LoadApplicationKeys();
//            if (!_applications.Any())
//            {
//                _logger.Error("Failed to find any applications in db.");
//                _noAppsWarningIsMade = true;
//                return;
//            }
//            _noAppsWarningIsMade = false;

//            while (reader.ReadNextInvalidFile(out headers, out json, FindSharedSecret))
//            {
//                var report = _reportDtoConverter.LoadReportFromJson(json);

//                var app = _applications.FirstOrDefault(x => x.ApplicationKey.Equals((string)headers["ApplicationKey"], StringComparison.OrdinalIgnoreCase));
//                if (app == null)
//                {
//                    _logger.Warn("Failed to identify application " + headers["ApplicationKey"]);
//                    continue;
//                    //TODO: FAIL
//                }

//                var newReport = _reportDtoConverter.ConvertReport(report, app.ApplicationId);
//                if (headers.ContainsKey("RemoteAddress"))
//                    newReport.RemoteAddress = (string)headers["RemoteAddress"];
//                var principal = new CoderrPrincipal(app.CustomerId, IAdoNetUnitOfWork.CreateDbName(app.CustomerId));
//                Thread.CurrentPrincipal = principal;
//                _scopedTaskInvoker.Execute<AnalyzeReport>(x => { x.Analyze(newReport); });
//            }
//        }

//        private string FindSharedSecret(string appKey)
//        {
//            var app = _applications.FirstOrDefault(x => x.ApplicationKey.Equals(appKey, StringComparison.OrdinalIgnoreCase));
//            return app == null ? null : app.SharedSecret;

//        }

//        private void LoadApplicationKeys()
//        {
//            using (var connection = OpenConnection("ReportsDB"))
//            using (var transaction = connection.BeginTransaction())
//            using (var cmd = connection.CreateCommand())
//            {
//                cmd.Transaction = transaction;
//                cmd.CommandText = @"SELECT CustomerId, ApplicationKey, SharedSecret, ApplicationId
//                                    FROM CustomerApplications";

//                try
//                {
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        var applications = new List<CustomerApp>();
//                        while (reader.Read())
//                        {
//                            var app = new CustomerApp
//                            {
//                                ApplicationId = (int)reader["ApplicationId"],
//                                CustomerId = (int)reader["CustomerId"],
//                                ApplicationKey = (string)reader["ApplicationKey"],
//                                SharedSecret = (string)reader["SharedSecret"]
//                            };
//                            applications.Add(app);
//                        }
//                        _applications = applications;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    throw cmd.CreateDataException(ex);
//                }
//            }
//        }

//        private IDbConnection OpenConnection(string name)
//        {
//            var conStr = ConfigurationManager.ConnectionStrings[name];
//            if (conStr == null)
//                throw new ConfigurationErrorsException("Failed to find connectionString '" + name + "'.");
//            var provider = DbProviderFactories.GetFactory(conStr.ProviderName);
//            if (provider == null)
//                throw new ConfigurationErrorsException("Failed to find DbProviderFactory '" + conStr.ProviderName + "'.");

//            var connection = provider.CreateConnection();
//            connection.ConnectionString = conStr.ConnectionString;
//            connection.Open();
//            return connection;
//        }
//    }
//}

