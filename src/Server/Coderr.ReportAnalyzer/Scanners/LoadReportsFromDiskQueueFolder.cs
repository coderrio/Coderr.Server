//TODO: Create MSMQ analyzer.
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
//using OneTrueError.MicroService.Core.Authentication;
//using OneTrueError.ReportAnalyzer.App.Services;
//using OneTrueError.Reporting.Contracts;

//namespace OneTrueError.ReportAnalyzer.App.Scanners
//{
//    [Component(Lifetime = Lifetime.Singleton)]
//    internal class LoadReportsFromDiskQueueFolder : ApplicationServiceTimer
//    {
//        private readonly ReportDtoConverter _reportDtoConverter = new ReportDtoConverter();
//        private readonly IScopedTaskInvoker _scopedTaskInvoker;
//        private List<CustomerApp> _applications = new List<CustomerApp>();

//        public LoadReportsFromDiskQueueFolder(IScopedTaskInvoker scopedTaskInvoker)
//        {
//            _scopedTaskInvoker = scopedTaskInvoker;
//        }

//        protected override void Execute()
//        {
//            var reader = new FileReader();
//            Dictionary<string, object> headers;
//            string json;
//            LoadApplicationKeys();
//            while (reader.ReadNextFile(out headers, out json))
//            {
//                ErrorReportDTO report;
//                try
//                {
//                    report = _reportDtoConverter.LoadReportFromJson(json);
//                }
//                catch (Exception)
//                {
//                    var path = ConfigurationManager.AppSettings["ReportStoragePath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
//                    var folder = Path.Combine(path, "InvalidFormat");
//                    File.WriteAllText(Path.Combine(folder, Guid.NewGuid().ToString() + ".json"), json);
//                    throw;
//                }


//                var app = _applications.FirstOrDefault(x => x.ApplicationKey.Equals((string)headers["ApplicationKey"], StringComparison.OrdinalIgnoreCase));
//                if (app == null)
//                {
//                    continue;
//                    //TODO: FAIL
//                }

//                var newReport = _reportDtoConverter.ConvertReport(report, app.ApplicationId);

//                if (headers.ContainsKey("RemoteAddress"))
//                    newReport.RemoteAddress = (string) headers["RemoteAddress"];

//                var principal = new OneTruePrincipal(app.CustomerId, "Service");
//                Thread.CurrentPrincipal = principal;
//                _scopedTaskInvoker.Execute<AnalyzeReport>(x => { x.Analyze(newReport); });
//            }
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
//                                ApplicationId = (int) reader["ApplicationId"],
//                                CustomerId = (int) reader["CustomerId"],
//                                ApplicationKey = (string) reader["ApplicationKey"]
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

