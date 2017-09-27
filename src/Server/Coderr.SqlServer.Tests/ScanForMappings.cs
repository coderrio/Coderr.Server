using System;
using Griffin.Data.Mapper;
using codeRR.SqlServer.Core.Users;

namespace codeRR.SqlServer.Tests
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