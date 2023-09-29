using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    internal class ApplicationGroupMapper : CrudEntityMapper<ApplicationGroup>
    {
        public ApplicationGroupMapper() : base("ApplicationGroups")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
        }
    }
}