using Coderr.Server.App.Core.Environments;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Environments
{
    public class EnvironmentMapper : CrudEntityMapper<Environment>
    {
        public EnvironmentMapper() : base("Environments")
        {
            Property(x => x.Id).PrimaryKey(true);
        }
    }
}
