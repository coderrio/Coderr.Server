using System;
using codeRR.Server.SqlServer.Core.Accounts;
using codeRR.Server.SqlServer.Tests.Helpers;
using Griffin.Data;
using Griffin.Data.Mapper;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests
{
    [Log]
    public class IntegrationTest : IDisposable
    {
        private static DatabaseManager _databaseManager;
        private TestDataManager _testDataManager;
        private static bool _isRun = false;
        private static readonly object _syncLock = new object();

        static IntegrationTest()
        {
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
            LogAttribute.Logger = output;
            _testDataManager = new TestDataManager(_databaseManager.OpenConnection);
        }

        public int FirstApplicationId => _testDataManager.ApplicationId;
        public int FirstUserId => _testDataManager.AccountId;

        public void Dispose()
        {
            Dispose(true);
        }

        protected IAdoNetUnitOfWork CreateUnitOfWork()
        {
            return _databaseManager.CreateUnitOfWork();
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