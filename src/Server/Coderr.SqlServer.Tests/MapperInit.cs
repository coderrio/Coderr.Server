using Xunit;

namespace codeRR.Server.SqlServer.Tests
{
    [CollectionDefinition(NAME)]
    public class MapperInit : ICollectionFixture<ScanForMappings>
    {
        public const string NAME = "MapperInit";
    }
}