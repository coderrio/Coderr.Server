using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Accounts;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace codeRR.Server.SqlServer.Core.Accounts
{
    [Component]
    public class AccountRepository : IAccountRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        public AccountRepository(IAdoNetUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            LogManager.GetLogger(typeof(AccountRepository)).Info("UOW hash: " + _uow.GetHashCode());
        }

        /// <inheritdoc />
        public Task<int> CountAsync()
        {
            return Task.FromResult((int)_uow.ExecuteScalar("SELECT CAST(count(*) as int) FROM Accounts"));
        }


        /// <inheritdoc />
        public async Task CreateAsync(Account account)
        {
            await _uow.InsertAsync(account);
        }

        /// <inheritdoc />
        public async Task<Account> FindByActivationKeyAsync(string activationKey)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE ActivationKey=@key";
                cmd.AddParameter("key", activationKey);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<Account> FindByUserNameAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Accounts WHERE UserName=@uname";
                cmd.AddParameter("uname", userName);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Id=@id";
                cmd.AddParameter("id", id);
                return await cmd.FirstAsync(new AccountMapper());
            }
        }

        /// <inheritdoc />
        public async Task<Account> FindByEmailAsync(string emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));

            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Email=@email";
                cmd.AddParameter("email", emailAddress);
                return await cmd.FirstOrDefaultAsync(new AccountMapper());
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Account>> GetByIdAsync(int[] ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE Id IN (@ids)";
                cmd.AddParameter("ids", string.Join(",", ids.Select(x => "'" + x + "'")));
                return await cmd.ToListAsync<Account>();
            }
        }


        /// <inheritdoc />
        public async Task<bool> IsEmailAddressTakenAsync(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT TOP 1 Email FROM Accounts WHERE Email = @Email";
                cmd.AddParameter("Email", email);
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }


        /// <inheritdoc />
        public async Task<bool> IsUserNameTakenAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));

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
            if (account == null) throw new ArgumentNullException(nameof(account));
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

        public async Task<Account> GetByUserNameAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Accounts WHERE UserName=@userName";
                cmd.AddParameter("userName", userName);
                return await cmd.FirstAsync(new AccountMapper());
            }
        }

    }
}