using Coderr.Server.App.Modules.Whitelists;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelist
{
    class WhitelistedDomainMapper : CrudEntityMapper<WhitelistedDomain>
    {
        public WhitelistedDomainMapper() : base("WhitelistedDomains")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.IpAddresses)
                .Ignore();
        }
    }
}
