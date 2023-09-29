using Coderr.Server.Abstractions.WorkItems;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.WorkItems
{
    class WorkItemMapper : CrudEntityMapper<WorkItemMapping>
    {
        public WorkItemMapper() : base("WorkItemMapping")
        {
        }
    }
}
