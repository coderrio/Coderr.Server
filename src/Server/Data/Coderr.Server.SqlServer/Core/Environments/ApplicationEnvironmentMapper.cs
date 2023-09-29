using Coderr.Server.App.Core.Environments;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Environments
{
    public class ApplicationEnvironmentMapper : CrudEntityMapper<ApplicationEnvironment>
    {
        public ApplicationEnvironmentMapper() : base("ApplicationEnvironments")
        {
            Property(x => x.Name).NotForCrud();
        }
    }
}