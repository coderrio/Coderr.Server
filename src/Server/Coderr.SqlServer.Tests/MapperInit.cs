using codeRR.SqlServer.Tests.Core.ApiKeys.Commands;
using Xunit;

namespace codeRR.SqlServer.Tests
{
    [CollectionDefinition(NAME)]
    public class MapperInit : ICollectionFixture<ScanForMappings>
    {
        public const string NAME = "MapperInit";
    }
}