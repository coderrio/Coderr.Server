using Coderr.Server.App.IncidentInsights.Subscribers;
using Coderr.Server.Domain.Modules.Statistics;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Insights
{
    public class IncidentProgressTrackerMapper : CrudEntityMapper<IncidentProgressTracker>
    {
        public IncidentProgressTrackerMapper() : base("CommonIncidentProgressTracking")
        {
            Property(x => x.IncidentId)
                .PrimaryKey(false);
        }
    }
}