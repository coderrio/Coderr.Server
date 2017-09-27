using Griffin.Data.Mapper;
using codeRR.App.Modules.Versions;
using codeRR.App.Modules.Versions.Events;

namespace codeRR.SqlServer.Modules.Versions
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