using System.Threading.Tasks;
using OneTrueError.App.Modules.Geolocation;
using OneTrueError.SqlServer.Modules.Geolocation;
using Xunit;

namespace OneTrueError.SqlServer.Tests.Modules.Geolocation
{
    public class ErrorOriginRepositoryTests
    {
        [Fact]
        public async Task Can_store_origin()
        {
            var origin = new ErrorOrigin("127.0.0.1", 934.934, 28.282);
            var uow = ConnectionFactory.Create();

            var handler = new ErrorOriginRepository(uow);
            await handler.CreateAsync(origin, 1, 2, 3);

            uow.Dispose();
        }
    }
}