using OneTrueError.SqlServer.Tests.Core.ApiKeys.Commands;
using Xunit;

namespace OneTrueError.SqlServer.Tests
{
    [CollectionDefinition(NAME)]
    public class MapperInit : ICollectionFixture<ScanForMappings>
    {
        public const string NAME = "MapperInit";
    }
}