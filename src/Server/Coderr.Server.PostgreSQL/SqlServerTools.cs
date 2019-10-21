using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Coderr.Server.Abstractions;
using Coderr.Server.Infrastructure;
using Coderr.Server.PostgreSQL.Migrations;
using Coderr.Server.PostgreSQL.Schema;

namespace Coderr.Server.PostgreSQL
{
    /// <summary>
    ///     MS Sql Server specific implementation of the database tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These tools should only be used during setup and updates.
    /// </para>
    /// </remarks>
    public class PostgreSQLTools : ISetupDatabaseTools
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly MigrationRunner _schemaManager;

        public PostgreSQLTools(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _schemaManager = new MigrationRunner(_connectionFactory, "Coderr", typeof(CoderrMigrationPointer).Namespace);
        }
        
        /// <summary>
        ///     Checks if the tables exists and are for the current DB schema.
        /// </summary>
        public bool IsTablesInstalled()
        {
            try
            {
                using (var con = _connectionFactory())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT OBJECT_ID(N'dbo.[Accounts]', N'U');";
                        var result = cmd.ExecuteScalar();

                        //null for SQL Express and DbNull for SQL Server
                        return result != null && !(result is DBNull);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Installation import = control over SQL")]
        public void CreateTables()
        {
            _schemaManager.Run();
        }

        IDbConnection ISetupDatabaseTools.OpenConnection()
        {
            return _connectionFactory();
        }

        public void TestConnection(string connectionString)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            con.Dispose();
        }
    }
}