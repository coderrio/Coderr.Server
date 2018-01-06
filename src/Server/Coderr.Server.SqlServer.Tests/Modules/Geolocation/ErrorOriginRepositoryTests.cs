using System;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Geolocation;
using codeRR.Server.SqlServer.Modules.Geolocation;
using Xunit;
using Xunit.Abstractions;

namespace codeRR.Server.SqlServer.Tests.Modules.Geolocation
{
    public class ErrorOriginRepositoryTests : IntegrationTest
    {
        private int _reportId;
        private int _incidentId;

        public ErrorOriginRepositoryTests(ITestOutputHelper helper) : base(helper)
        {
            ResetDatabase();
            CreateReportAndIncident(out _reportId, out _incidentId);
        }

        [Fact]
        public async Task Can_store_origin()
        {
            var origin = new ErrorOrigin("127.0.0.1", 934.934, 28.282);
            using (var uow = CreateUnitOfWork())
            {
                var handler = new ErrorOriginRepository(uow);
                await handler.CreateAsync(origin, FirstApplicationId, _incidentId, _reportId);
                uow.SaveChanges();
            }
        }
        
    }
}