using Griffin.Data.Mapper;
using OneTrueError.App.Core.Users;

namespace OneTrueError.SqlServer.Core.Users
{
    public class UserMapper : CrudEntityMapper<User>
    {
        public UserMapper() : base("Users")
        {
            Property(x => x.AccountId).PrimaryKey(false);
        }
    }
}