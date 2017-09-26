using System;
using Griffin.Data.Mapper;
using OneTrueError.SqlServer.Core.Users;

namespace OneTrueError.SqlServer.Tests
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