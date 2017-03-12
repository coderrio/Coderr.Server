using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Versions;
using OneTrueError.App.Modules.Versions.Events;

namespace OneTrueError.SqlServer.Modules.Versions
{
    public class ApplicationVersionMonthMapper : CrudEntityMapper<ApplicationVersionMonth>
    {
        public ApplicationVersionMonthMapper() : base("ApplicationVersionMonths")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
        }
    }
}