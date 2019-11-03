using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    public class WhitelistedDomainApplicationMapper : CrudEntityMapper<WhitelistedDomainApplication>
    {
        public WhitelistedDomainApplicationMapper() : base("WhitelistedDomainApps")
        {
        }
    }
}