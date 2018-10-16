using Coderr.Server.App.Modules.Versions;
using Coderr.Server.Domain.Modules.ApplicationVersions;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Versions
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