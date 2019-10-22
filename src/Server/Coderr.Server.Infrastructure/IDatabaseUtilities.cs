using System.Data;
using System.Security.Claims;

namespace Coderr.Server.Infrastructure
{
    public interface ISetupDatabaseTools
    {
        /// <summary>
        ///     Create all tables in the new DB.
        /// </summary>
        void CreateTables();

        /// <summary>
        ///     Checks if the tables exists and are for the current DB schema.
        /// </summary>
        bool IsTablesInstalled();

        /// <summary>
        ///     Open a new connection.
        /// </summary>
        /// <returns>Connection</returns>
        IDbConnection OpenConnection();
        IDbConnection GetConnection(string connectionString, ClaimsPrincipal arg);
        void TestConnection(string connectionString);
    }
}