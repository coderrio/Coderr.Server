using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.SqlServer.Insights.Applications;
using Xunit;

namespace Coderr.Server.SqlServer.Tests.Insights.Applications
{
    public class NewIncidentsIndicatorTests : SimpleDbTest
    {
        public NewIncidentsIndicatorTests()
        {
            FillCommonIncidentData();
        }

        [Fact]
        public async Task Should_not_give_mapping_errors()
        {
            var items = new Dictionary<int, List<KeyPerformanceIndicator>>();
            var context = new KeyPerformanceIndicatorContext(items) {ApplicationIds = new[] {1}};


            var sut = new NewIncidentsIndicator(OpenUow());
            await sut.CollectAsync(context);

            var kpi = items.Values.First();
            
        }

    }
}
