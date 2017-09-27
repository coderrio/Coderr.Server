using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Core.Users
{
    /// <summary>
    ///     User repository
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>task</returns>
        Task CreateAsync(User user);

        /// <summary>
        ///     Find user by email
        /// </summary>
        /// <param name="emailAddress">email address</param>
        /// <returns>user if found; otherwise <c>null</c>.</returns>
        Task<User> FindByEmailAsync(string emailAddress);

        /// <summary>
        ///     Get user by account id
        /// </summary>
        /// <param name="accountId">account id</param>
        /// <returns>user</returns>
        /// <exception cref="EntityNotFoundException">user was not found</exception>
        Task<User> GetUserAsync(int accountId);

        /// <summary>
        ///     Update user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>task</returns>
        Task UpdateAsync(User user);
    }
}