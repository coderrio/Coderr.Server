using Coderr.Server.Api.Modules.Triggers;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.Triggers
{
    public class TriggerDtoMapper : EntityMapper<TriggerDTO>
    {
        public TriggerDtoMapper()
        {
            Property(x => x.Id)
                .ToPropertyValue(x => x.ToString());

            Property(x => x.Summary)
                .Ignore();
        }
    }
}