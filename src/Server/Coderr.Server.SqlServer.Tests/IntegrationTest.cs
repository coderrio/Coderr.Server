using System;
using System.Data;
using System.IO;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.SqlServer.Core.Accounts;
using codeRR.Server.SqlServer.Tests.Helpers;
using codeRR.Server.SqlServer.Tests.Models;
using codeRR.Server.SqlServer.Tests.Xunit;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;
using log4net.Config;
using Xunit;
using Xunit.Abstractions;
[assembly: TestFramework("codeRR.Server.SqlServer.Tests.Xunit.XunitTestFrameworkWithAssemblyFixture", "codeRR.Server.SqlServer.Tests")]

namespace codeRR.Server.SqlServer.Tests
{
    public class IntegrationTest : IDisposable
    {
        private static DatabaseManager _databaseManager;
        private TestDataManager _testDataManager;
        private static bool _isRun = false;
        private static readonly object _syncLock = new object();

        static IntegrationTest()
        {
            var path2 = AppDomain.CurrentDomain.BaseDirectory;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(path2, "log4net.config")));
            var logger = LogManager.GetLogger(typeof(IntegrationTest));
            logger.Info("Loaded");

            

            AppDomain.CurrentDomain.DomainUnload += (o, e) =>
            {
                if (_databaseManager == null)
                    return;
                _databaseManager.Dispose();
                _databaseManager = null;
            };
            lock (_syncLock)
            {
                _databaseManager = new DatabaseManager();
                _databaseManager.CreateEmptyDatabase();
                _databaseManager.InitSchema();
            }

            var mapper = new AssemblyScanningMappingProvider();
            mapper.Scan(typeof(AccountRepository).Assembly);
            EntityMappingProvider.Provider = mapper;
        }

        public IntegrationTest(ITestOutputHelper output)
        {
            _testDataManager = new TestDataManager(_databaseManager.OpenConnection);
            _testDataManager.TestUser = new TestUser()
            {
                Email = "test@somewhere.com",
                Password = "123456",
                Username = "admin"
            };
        }

        public int FirstApplicationId => _testDataManager.ApplicationId;
        public int FirstUserId => _testDataManager.AccountId;

        public void Dispose()
        {
            Dispose(true);
        }

        protected OurUnitOfWork CreateUnitOfWork()
        {
            return _databaseManager.CreateUnitOfWork();
        }

        protected IDbConnection OpenConnection()
        {
            return _databaseManager.OpenConnection();
        }


        protected virtual void Dispose(bool isBeingDisposed)
        {
        }

        protected void CreateReportAndIncident(out int reportId, out int incidentId)
        {
            _testDataManager.CreateReportAndIncident(out reportId, out incidentId);
        }

        protected void ResetDatabase(string baseUrl = "http://localhost:53844")
        {
            _testDataManager.ResetDatabase(baseUrl);
        }
    }
}