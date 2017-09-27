using codeRR.Server.Api.Modules.Triggers;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Triggers
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