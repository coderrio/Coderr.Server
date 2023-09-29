using Coderr.Server.App.WorkItems;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.WorkItems
{
    class WorkItemIntegrationMappingMapper : CrudEntityMapper<WorkItemIntegrationMapping>
    {
        public WorkItemIntegrationMappingMapper() : base("WorkItemIntegrationMapping")
        {
            Property(x => x.ApplicationId).PrimaryKey(false);
        }
    }
}
