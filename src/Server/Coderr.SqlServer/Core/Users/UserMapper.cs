using Griffin.Data.Mapper;
using codeRR.App.Core.Users;

namespace codeRR.SqlServer.Core.Users
{
    public class UserMapper : CrudEntityMapper<User>
    {
        public UserMapper() : base("Users")
        {
            Property(x => x.AccountId).PrimaryKey(false);
        }
    }
}