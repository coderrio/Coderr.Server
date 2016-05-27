using System.Data;

namespace OneTrueError.Infrastructure
{
    public interface ISetupDatabaseTools
    {
        /// <summary>
        /// Used to check if the given connection string actually works
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="DataException">Something do not work</exception>
        void CheckConnectionString(string connectionString);

        /// <summary>
        /// Create all tables in the new DB.
        /// </summary>
        void CreateTables();

        /// <summary>
        /// Open a new connection.
        /// </summary>
        /// <returns>Connection</returns>
        IDbConnection OpenConnection();

        /// <summary>
        /// Checks if the tables exists and are for the current DB schema.
        /// </summary>
        bool GotUpToDateTables();
    }
}
