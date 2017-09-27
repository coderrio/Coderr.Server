using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using codeRR.App.Core.Accounts;

namespace codeRR.SqlServer.Core.Accounts
{
    [Component]
    public class AccountRepository : IAccountRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public AccountRepository(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");
            _uow = uow;
        }

        /*
        public string Id { get; private set; }
        public string UserName { get; private set; }
        public string HashedPassword { get; private set; }
        public string Salt { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public AccountState AccountState { get; set; }
        public string Email { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string CompanyName { get; set; }
        public string LastUsedOrganization { get; set; }
        public string ActivationKey { get; set; }
        public int LoginAttempts { get; private set; }
        public DateTime LastLoginAtUtc { get; private set; }*/

        public async Task CreateAsync(Account account)
        {
            await _uow.InsertAsync(account);
        }

        public async Task<Account> FindByActivationKeyAsync(string activationKey)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE ActivationKey=@key";
                cmd.AddParameter("key", activationKey);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        public async Task UpdateAsync(Account account)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText =
                    "UPDATE Accounts SET " +
                    " Username = @Username, " +
                    " HashedPassword = @HashedPassword, " +
                    " Salt = @Salt, " +
                    " CreatedAtUtc = @CreatedAtUtc, " +
                    " AccountState = @AccountState, " +
                    " Email = @Email, " +
                    " UpdatedAtUtc = @UpdatedAtUtc, " +
                    " ActivationKey = @ActivationKey, " +
                    " LoginAttempts = @LoginAttempts, " +
                    " LastLoginAtUtc = @LastLoginAtUtc " +
                    "WHERE Id = @Id";
                cmd.AddParameter("@Id", account.Id);
                cmd.AddParameter("@Username", account.UserName);
                cmd.AddParameter("@HashedPassword", account.HashedPassword);
                cmd.AddParameter("@Salt", account.Salt);
                cmd.AddParameter("@CreatedAtUtc", account.CreatedAtUtc);
                cmd.AddParameter("@AccountState", account.AccountState.ToString());
                cmd.AddParameter("@Email", account.Email);
                cmd.AddParameter("@UpdatedAtUtc",
                    account.UpdatedAtUtc == DateTime.MinValue ? (object) null : account.UpdatedAtUtc);
                cmd.AddParameter("@ActivationKey", account.ActivationKey);
                cmd.AddParameter("@LoginAttempts", account.LoginAttempts);
                cmd.AddParameter("@LastLoginAtUtc",
                    account.LastLoginAtUtc == DateTime.MinValue ? (object) null : account.LastLoginAtUtc);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<Account> FindByUserNameAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Accounts WHERE UserName=@uname";
                cmd.AddParameter("uname", userName);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentNullException("id");
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Id=@id";
                cmd.AddParameter("id", id);
                return await cmd.FirstAsync(new AccountMapper());
            }
        }

        public async Task<Account> FindByEmailAsync(string emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Email=@email";
                cmd.AddParameter("email", emailAddress);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        public async Task<IEnumerable<Account>> GetByIdAsync(int[] ids)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Id IN (@ids)";
                cmd.AddParameter("ids", string.Join(",", ids.Select(x => "'" + x + "'")));
                return await cmd.ToListAsync<Account>();
            }
        }


        public async Task<bool> IsEmailAddressTakenAsync(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT TOP 1 Email FROM Accounts WHERE Email = @Email";
                cmd.AddParameter("Email", email);
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }


        public async Task<bool> IsUserNameTakenAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT TOP 1 UserName FROM Accounts WHERE UserName = @userName";
                cmd.AddParameter("userName", userName);
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }

        public void Create(Account account)
        {
            using (var cmd = _uow.CreateCommand())
            {
                //for systems where ID must be specified
                if (account.Id > 0)
                {
                    cmd.CommandText =
                        "INSERT INTO Accounts (Id, Username, HashedPassword, Salt, CreatedAtUtc, AccountState, Email, UpdatedAtUtc, ActivationKey, LoginAttempts, LastLoginAtUtc) " +
                        " VALUES(@Id, @Username, @HashedPassword, @Salt, @CreatedAtUtc, @AccountState, @Email, @UpdatedAtUtc, @ActivationKey, @LoginAttempts, @LastLoginAtUtc); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.AddParameter("id", account.Id);

                }
                else
                {
                    cmd.CommandText =
                        "INSERT INTO Accounts (Username, HashedPassword, Salt, CreatedAtUtc, AccountState, Email, UpdatedAtUtc, ActivationKey, LoginAttempts, LastLoginAtUtc) " +
                        " VALUES(@Username, @HashedPassword, @Salt, @CreatedAtUtc, @AccountState, @Email, @UpdatedAtUtc, @ActivationKey, @LoginAttempts, @LastLoginAtUtc); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                }
                cmd.AddParameter("@Username", account.UserName);
                cmd.AddParameter("@HashedPassword", account.HashedPassword);
                cmd.AddParameter("@Salt", account.Salt);
                cmd.AddParameter("@CreatedAtUtc", account.CreatedAtUtc);
                cmd.AddParameter("@AccountState", account.AccountState.ToString());
                cmd.AddParameter("@Email", account.Email);
                cmd.AddParameter("@UpdatedAtUtc",
                    account.UpdatedAtUtc == DateTime.MinValue ? (object) null : account.UpdatedAtUtc);
                cmd.AddParameter("@ActivationKey", account.ActivationKey);
                cmd.AddParameter("@LoginAttempts", account.LoginAttempts);
                cmd.AddParameter("@LastLoginAtUtc",
                    account.LastLoginAtUtc == DateTime.MinValue ? (object) null : account.LastLoginAtUtc);

                if (account.Id > 0)
                {
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    var value = (int) cmd.ExecuteScalar();
                    account.GetType().GetProperty("Id").SetValue(account, value);
                }
            }
        }

        public Account GetByUserName(string userName)
        {
            return _uow.First<Account>(new {UserName = userName});
        }
    }
}