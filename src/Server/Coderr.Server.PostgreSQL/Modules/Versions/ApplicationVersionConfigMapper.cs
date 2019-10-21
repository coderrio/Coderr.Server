using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.Versions
{
    public class ApplicationVersionConfigMapper : CrudEntityMapper<ApplicationVersionConfig>
    {
        public ApplicationVersionConfigMapper() : base("ApplicationVersions")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
        }
    }
}