using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Settings
{
    public class AccountSettingMapper : CrudEntityMapper<AccountSetting>
    {
        public AccountSettingMapper():base("UserSettings")
        {
            Property(x => x.AccountId).PrimaryKey(false);
        }

    }
}