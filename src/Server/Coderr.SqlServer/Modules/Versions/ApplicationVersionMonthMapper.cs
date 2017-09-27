using codeRR.Server.App.Modules.Versions;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.Versions
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