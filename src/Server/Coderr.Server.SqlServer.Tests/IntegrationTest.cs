using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Coderr.Server.SqlServer.Core.Accounts;
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
        private static bool Inited;
        public static Action<IDbConnection> SchemaUpdater;
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

            var mapper = EntityMappingProvider.Provider as AssemblyScanningMappingProvider;
            if (mapper == null)
            {
                mapper = new AssemblyScanningMappingProvider();
                EntityMappingProvider.Provider = mapper;
            }

            mapper.Scan(typeof(AccountRepository).Assembly);
        }


        public IntegrationTest(ITestOutputHelper output)
        {
            lock (SyncLock)
            {
                if (!Inited)
                {
                    _databaseManager = new DatabaseManager();
                    _databaseManager.CreateEmptyDatabase();
                    _databaseManager.InitSchema();
                    if (SchemaUpdater != null)
                        using (var connection = _databaseManager.OpenConnection())
                        {
                            SchemaUpdater(connection);
                        }

                    DeleteOldTestDatabases();

                    Inited = true;
                }
            }

            _testDataManager = new TestDataManager(_databaseManager.OpenConnection)
            {
                TestUser = new TestUser
                {
                    Email = "test@somewhere.com",
                    Password = "123456",
                    Username = "admin"
                }
            };
        }

        public int FirstApplicationId => _testDataManager.ApplicationId;
        public int FirstUserId => _testDataManager.AccountId;

        public void Dispose()
        {
            Dispose(true);
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

        private static void DeleteOldTestDatabases()
        {
            var todayDbStr = "coderrTest" + DateTime.Today.ToString("MMdd");
            using (var con = _databaseManager.OpenConnection())
            {
                con.ChangeDatabase("master");
                List<string> databasesToDelete = new List<string>();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sys.databases where name like 'coderr%'";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dbName = reader.GetString(0);
                            if (!dbName.StartsWith(todayDbStr))
                                databasesToDelete.Add(dbName);
                        }
                    }
                }

                foreach (var dbName in databasesToDelete)
                {
                    try
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "DROP DATABASE " + dbName;
                            cmd.CommandTimeout = 10;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (DataException exception)
                    {
                        Console.WriteLine("Failed to delete " + dbName + "\r\nException: " + exception);
                    }
                }
            }
            Console.WriteLine("DB cleanup done.");
        }
    }
}