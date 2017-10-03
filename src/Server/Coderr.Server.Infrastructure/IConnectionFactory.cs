using System.Data;

namespace codeRR.Server.Infrastructure
{
    /// <summary>
    ///     Used to open connections to the database
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        ///     Open standard connection
        /// </summary>
        /// <returns></returns>
        IDbConnection Open();

        /// <summary>
        ///     Open named connection
        /// </summary>
        /// <param name="connectionStringName">Name from the web.config connectionStrings section</param>
        /// <returns>connection</returns>
        /// <exception cref="DataException">Failed to open connection</exception>
        IDbConnection Open(string connectionStringName);

        /// <summary>
        ///     Open named connection
        /// </summary>
        /// <param name="connectionStringName">Name from the web.config connectionStrings section</param>
        /// <returns>connection if name exists; otherwise <c>null</c>.</returns>
        /// <exception cref="DataException">Failed to open connection (if name existed)</exception>
        IDbConnection TryOpen(string connectionStringName);
    }
}