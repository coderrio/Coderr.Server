using Coderr.Server.Domain.Core.User;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Users
{
    public class UserMapper : CrudEntityMapper<User>
    {
        public UserMapper() : base("Users")
        {
            Property(x => x.AccountId).PrimaryKey(false);
        }
    }
}