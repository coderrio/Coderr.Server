using codeRR.Server.App.Core.Users;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Users
{
    public class UserMapper : CrudEntityMapper<User>
    {
        public UserMapper() : base("Users")
        {
            Property(x => x.AccountId).PrimaryKey(false);
        }
    }
}