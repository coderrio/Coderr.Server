using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Server.WebSite.Models;
using Griffin.Data;
using Microsoft.AspNetCore.Identity;

namespace Coderr.Server.WebSite.Identity
{
    public class UserStore : IUserStore<ApplicationUser>
    {
        private IAdoNetUnitOfWork _dbTransaction;

        public UserStore(IAdoNetUnitOfWork dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public void Dispose()
        {
        }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var cmd = _dbTransaction.CreateDbCommand())
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    cmd.CommandText = "SELECT Id FROM Accounts WHERE UserName = @userName";
                    cmd.AddParameter("userName", user.UserName);
                }
                else
                {
                    cmd.CommandText = "SELECT Id FROM Accounts WHERE Email = @email";
                    cmd.AddParameter("email", user.Email);
                }
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                return result is DBNull ? null : result.ToString();
            }
        }

        public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var cmd = _dbTransaction.CreateDbCommand())
            {
                if (!string.IsNullOrEmpty(user.Id))
                {
                    cmd.CommandText = "SELECT UserName FROM Accounts WHERE Id = @id";
                    cmd.AddParameter("id", user.Id);
                }
                else
                {
                    cmd.CommandText = "SELECT UserName FROM Accounts WHERE Email = @email";
                    cmd.AddParameter("email", user.Email);
                }
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                return result is DBNull ? null : result.ToString();
            }
        }

        public async Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentNullException("user.Id");

            using (var cmd1 = _dbTransaction.CreateDbCommand())
            {
                cmd1.CommandText = "UPDATE Users SET UserName=@userName WHERE AccountId = @id";
                cmd1.AddParameter("id", user.Id);
                await cmd1.ExecuteScalarAsync(cancellationToken);
            }

            using (var cmd2 = _dbTransaction.CreateDbCommand())
            {
                cmd2.CommandText = "UPDATE Accounts SET UserName=@userName WHERE Id = @id";
                cmd2.AddParameter("id", user.Id);
                await cmd2.ExecuteScalarAsync(cancellationToken);
            }
        }

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return (await GetUserNameAsync(user, cancellationToken))?.ToUpper();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var cmd = (DbCommand)_dbTransaction.CreateDbCommand())
            {
                cmd.CommandText = "INSERT INTO Users (AccountId, UserName, EmailAddress, HashedPassword) VALUES(@id, @userName, @email, @passwordHash)";
                cmd.AddParameter("id", int.Parse(user.Id));
                cmd.AddParameter("userName", user.UserName);
                cmd.AddParameter("email", user.Email);
                cmd.AddParameter("passwordHash", user.PasswordHash);
                //TODO: Not using our salt.
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return new IdentityResult {Errors = { }};

            //TODO: User
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var cmd = _dbTransaction.CreateDbCommand())
            {
                    cmd.CommandText = "SELECT Id, UserName, Email, HashedPassword, LoginAttempts FROM Accounts WHERE Id = @id";
                    cmd.AddParameter("id", userId);
                    using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        if (!await reader.ReadAsync(cancellationToken))
                            return null;

                        var user = new ApplicationUser
                        {
                            Id = reader.GetInt32(0).ToString(),
                            UserName = reader.GetString(1),
                            NormalizedUserName = reader.GetString(1).ToUpper(),
                            Email = reader.GetString(2),
                            NormalizedEmail = reader.GetString(2),
                            PasswordHash = reader.GetString(3),
                            AccessFailedCount = reader.GetInt32(4)
                        };
                        return user;
                    }
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var cmd = _dbTransaction.CreateDbCommand())
            {
                cmd.CommandText = "SELECT Id, UserName, Email, HashedPassword, LoginAttempts FROM Accounts WHERE UserName = @userName";
                cmd.AddParameter("userName", normalizedUserName);
                using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                        return null;

                    var user = new ApplicationUser
                    {
                        Id = reader.GetInt32(0).ToString(),
                        UserName = reader.GetString(1),
                        NormalizedUserName = reader.GetString(1).ToUpper(),
                        Email = reader.GetString(2),
                        NormalizedEmail = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        AccessFailedCount = reader.GetInt32(4)
                    };
                    return user;
                }
            }
        }
    }
}