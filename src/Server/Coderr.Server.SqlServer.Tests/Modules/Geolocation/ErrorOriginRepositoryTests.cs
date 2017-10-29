using System;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Geolocation;
using codeRR.Server.SqlServer.Modules.Geolocation;
using Xunit;

namespace codeRR.Server.SqlServer.Tests.Modules.Geolocation
{
    [Collection(MapperInit.NAME)]
    public class ErrorOriginRepositoryTests : IDisposable
    {
        private readonly TestTools _testTools = new TestTools();

        public ErrorOriginRepositoryTests()
        {
            _testTools.CreateDatabase();
            _testTools.ToLatestVersion();
        }

        [Fact]
        public async Task Can_store_origin()
        {
            var origin = new ErrorOrigin("127.0.0.1", 934.934, 28.282);
            var uow = _testTools.CreateUnitOfWork();
            _testTools.CreateBasicData();

            var handler = new ErrorOriginRepository(uow);
            await handler.CreateAsync(origin, 1, 1, 1);

            uow.Dispose();
        }

        public void Dispose()
        {
            _testTools?.Dispose();
        }
    }
}