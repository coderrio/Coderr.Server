using Griffin.Data.Mapper;
using OneTrueError.Api.Modules.Triggers;

namespace OneTrueError.SqlServer.Modules.Triggers
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