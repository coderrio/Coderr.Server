using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using Coderr.Server.Abstractions;
using Coderr.Server.SqlServer.Core.Accounts;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.SqlServer.Tests.Helpers;
using Coderr.Server.SqlServer.Tests.Models;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;
using log4net.Config;
using Xunit;
using Xunit.Abstractions;

[assembly:
    TestFramework("Coderr.Server.SqlServer.Tests.Xunit.XunitTestFrameworkWithAssemblyFixture",
        "Coderr.Server.SqlServer.Tests")]

namespace Coderr.Server.SqlServer.Tests
{
    public class IntegrationTest : IDisposable
    {
        protected static DatabaseManager _databaseManager;
        private static readonly object SyncLock = new object();
        private static bool _inited;
        public static Action<IDbConnection> SchemaUpdater;

        protected static List<(string name, string scriptNamespace)> MigrationSources =
            new List<(string name, string scriptNamespace)>();

        public static AssemblyScanningMappingProvider MappingProvider =
            (AssemblyScanningMappingProvider)EntityMappingProvider.Provider;

        private readonly TestDataManager _testDataManager;

        static IntegrationTest()
        {
            var path2 = AppDomain.CurrentDomain.BaseDirectory;
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(Path.Combine(path2, "log4net.config")));
            var logger = LogManager.GetLogger(typeof(IntegrationTest));
            logger.Info("Loaded");


            AppDomain.CurrentDomain.DomainUnload += (o, e) =>
            {
                if (_databaseManager == null)
                    return;
                _databaseManager.Dispose();
                _databaseManager = null;
            };

            EntityMappingProvider.Provider = new AssemblyScanningMappingProvider();
            MappingProvider.Scan(typeof(AccountRepository).Assembly);
        }

        public IntegrationTest(ITestOutputHelper output)
        {
            lock (SyncLock)
            {
                if (!_inited)
                {
                    foreach (var source in MigrationSources)
                        SqlServerTools.AddMigrationRunner(new MigrationRunner(OpenConnection, source.name,
                            source.scriptNamespace));

                    _databaseManager = new DatabaseManager("CoderrTest");
                    _databaseManager.CreateDatabase();
                    _databaseManager.InitSchema();
                    if (SchemaUpdater != null)
                        using (var connection = _databaseManager.OpenConnection())
                        {
                            SchemaUpdater(connection);
                        }

                    _inited = true;
                }
            }

            _testDataManager = new TestDataManager(_databaseManager.OpenConnection)
            {
                TestUser = new TestUser {Email = "test@somewhere.com", Password = "123456", Username = "admin"}
            };
        }

        public int FirstApplicationId => _testDataManager.ApplicationId;
        public int FirstUserId => _testDataManager.AccountId;

        public void Dispose()
        {
            Dispose(true);
            _databaseManager.Dispose();
        }


        protected int CreateApplication(string applicationName, int accountIdForAdmin)
        {
            return _testDataManager.CreateApplication(applicationName, accountIdForAdmin);
        }

        protected void CreateReportAndIncident(out int reportId, out int incidentId)
        {
            _testDataManager.CreateReportAndIncident(out reportId, out incidentId);
        }

        protected void CreateReportAndIncident(int applicationId, out int reportId, out int incidentId)
        {
            _testDataManager.CreateReportAndIncident(applicationId, out reportId, out incidentId);
        }

        protected IAdoNetUnitOfWork CreateUnitOfWork()
        {
            return _databaseManager.CreateUnitOfWork();
        }


        protected virtual void Dispose(bool isBeingDisposed)
        {
        }

        protected IDbConnection OpenConnection()
        {
            return _databaseManager.OpenConnection();
        }

        protected void ResetDatabase(string baseUrl = "http://localhost:53844")
        {
            _testDataManager.ResetDatabase(baseUrl);
        }

    }
}