using Griffin.Data.Mapper;
using codeRR.Api.Modules.Triggers;

namespace codeRR.SqlServer.Modules.Triggers
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