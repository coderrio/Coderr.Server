using Coderr.Server.Common.AzureDevOps.App.Connections;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Azure.DevOps
{
    class SettingsMapper : CrudEntityMapper<Settings>
    {
        public SettingsMapper() : base("AzureDevOpsSettings")
        {
            Property(x => x.ApplicationId).PrimaryKey(false);
        }
    }
}
