using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Core.Accounts
{
    /// <summary>
    ///     Repository for accounts
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        ///     Count the number of created accounts.
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        ///     Create a new account.
        /// </summary>
        /// <param name="account">account</param>
        /// <returns>task</returns>
        /// <remarks>
        ///     <para>UserName and email address must be unique</para>
        /// </remarks>
        Task CreateAsync(Account account);

        /// <summary>
        ///     find by using the activation key
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns>account if found; otherwise <c>null</c>.</returns>
        Task<Account> FindByActivationKeyAsync(string activationKey);

        /// <summary>
        ///     find user by using email.
        /// </summary>
        /// <param name="emailAddress">email</param>
        /// <returns>account if found; otherwise <c>null</c>.</returns>
        Task<Account> FindByEmailAsync(string emailAddress);

        /// <summary>
        ///     Find user
        /// </summary>
        /// <param name="userName">user name to match</param>
        /// <returns>account if found; otherwise <c>null</c>.</returns>
        Task<Account> FindByUserNameAsync(string userName);

        /// <summary>
        ///     Get account by id
        /// </summary>
        /// <param name="id">account id</param>
        /// <returns>account</returns>
        /// <exception cref="EntityNotFoundException">No account exists with the given id.</exception>
        Task<Account> GetByIdAsync(int id);

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>user</returns>
        /// <exception cref="EntityNotFoundException">No account exists with the given userName.</exception>
        Task<Account> GetByUserNameAsync(string userName);

        /// <summary>
        ///     Get all accounts by the given ids
        /// </summary>
        /// <param name="ids">account ids</param>
        /// <returns>Corresponding accounts</returns>
        /// <exception cref="EntityNotFoundException">One or more of the given ids did not have a matching account..</exception>
        Task<IEnumerable<Account>> GetByIdAsync(int[] ids);

        /// <summary>
        ///     Check if email address is taken
        /// </summary>
        /// <param name="email">email</param>
        /// <returns><c>true</c> if it exists; otherwise <c>false</c>.</returns>
        Task<bool> IsEmailAddressTakenAsync(string email);


        /// <summary>
        ///     Check if user name is already taken.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns><c>true</c> if it exists; otherwise <c>false</c>.</returns>
        Task<bool> IsUserNameTakenAsync(string userName);

        /// <summary>
        ///     Update account
        /// </summary>
        /// <param name="account">account</param>
        /// <returns>task</returns>
        Task UpdateAsync(Account account);
    }
}