using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    internal class ApplicationGroupMapMapper : CrudEntityMapper<ApplicationGroupMap>
    {
        public ApplicationGroupMapMapper() : base("ApplicationGroupMap")
        {
            Property(x => x.Id).PrimaryKey(false);
        }
    }
}