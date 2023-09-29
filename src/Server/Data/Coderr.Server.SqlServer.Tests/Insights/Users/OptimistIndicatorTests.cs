using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.SqlServer.Insights.Users;
using Xunit;

namespace Coderr.Server.SqlServer.Tests.Insights.Users
{
    public class OptimistIndicatorTests : SimpleDbTest
    {
        public OptimistIndicatorTests()
        {
            FillCommonIncidentData();
        }

        [Fact]
        public async Task Should_not_give_mapping_errors()
        {
            var items = new Dictionary<int, List<KeyPerformanceIndicator>>();
            var context = new KeyPerformanceIndicatorContext(items) {ApplicationIds = new[] {1}};


            var sut = new ProcastinatorIndicator(OpenUow());
            await sut.CollectAsync(context);

            var kpi = items.Values.First();

        }
    }
}
