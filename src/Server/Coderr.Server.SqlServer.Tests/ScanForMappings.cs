using System;
using codeRR.Server.SqlServer.Core.Users;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Tests
{
    public class ScanForMappings : IDisposable
    {
        public ScanForMappings()
        {
            var provider = new AssemblyScanningMappingProvider();
            provider.Scan(typeof(UserMapper).Assembly);
            EntityMappingProvider.Provider = provider;
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}