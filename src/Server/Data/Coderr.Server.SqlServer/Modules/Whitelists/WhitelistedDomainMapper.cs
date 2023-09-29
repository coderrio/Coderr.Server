using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    class WhitelistMapper : CrudEntityMapper<App.Modules.Whitelists.Whitelist>
    {
        public WhitelistMapper() : base("WhitelistedDomains")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.IpAddresses)
                .Ignore();
            Property(x => x.ApplicationIds)
                .Ignore();
        }
    }
}
