using System.Data.Common;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Users;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Users
{
    [Component]
    public class UserRepository : IUserRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public UserRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task CreateAsync(User user)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (AccountId, UserName, EmailAddress) VALUES(@id, @userName, @email)";
                cmd.AddParameter("id", user.AccountId);
                cmd.AddParameter("userName", user.UserName);
                cmd.AddParameter("email", user.EmailAddress);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<User> GetUserAsync(int accountId)
        {
            return await _uow.FirstAsync<User>(new {AccountId = accountId});
        }

        public async Task UpdateAsync(User user)
        {
            await _uow.UpdateAsync(user);
        }

        public async Task<User> FindByEmailAsync(string emailAddress)
        {
            return await _uow.FirstOrDefaultAsync<User>("EmailAddress = @1", emailAddress);
        }
    }
}