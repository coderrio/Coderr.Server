using System;
using System.Data;

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

        void TestConnection(string connectionString);
        bool IsConfigurationComplete(string connectionString);
        void MarkConfigurationAsComplete();
    }
}