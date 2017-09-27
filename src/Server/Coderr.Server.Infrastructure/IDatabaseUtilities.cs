using System.Data;

namespace codeRR.Server.Infrastructure
{
    public interface ISetupDatabaseTools
    {
        /// <summary>
        /// Check if the current DB schema is out of date compared to the embedded schema resources.
        /// </summary>
        bool CanSchemaBeUpgraded();
        
        /// <summary>
        /// Update DB schema to latest version.
        /// </summary>
        void UpgradeDatabaseSchema();

        /// <summary>
        ///     Used to check if the given connection string actually works
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="DataException">Something do not work</exception>
        void CheckConnectionString(string connectionString);

        /// <summary>
        ///     Create all tables in the new DB.
        /// </summary>
        void CreateTables();

        /// <summary>
        ///     Checks if the tables exists and are for the current DB schema.
        /// </summary>
        bool GotUpToDateTables();

        /// <summary>
        ///     Open a new connection.
        /// </summary>
        /// <returns>Connection</returns>
        IDbConnection OpenConnection();
    }
}