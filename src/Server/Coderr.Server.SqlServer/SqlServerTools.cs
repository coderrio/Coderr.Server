using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Coderr.Server.Abstractions;
using Coderr.Server.Infrastructure;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.SqlServer.Schema;

namespace Coderr.Server.SqlServer
{
    /// <summary>
    ///     MS Sql Server specific implementation of the database tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These tools should only be used during setup and updates.
    /// </para>
    /// </remarks>
    public class SqlServerTools : ISetupDatabaseTools
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly MigrationRunner _schemaManager;

        public SqlServerTools(string connectionString, ClaimsPrincipal arg)
        {
            _connectionFactory = () =>
            {
                return this.GetConnection(connectionString, arg);
            };
            _schemaManager = new MigrationRunner(_connectionFactory, "Coderr", typeof(CoderrMigrationPointer).Namespace);
        }
        public SqlServerTools(Func<IDbConnection> connectionFactory)
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
                        cmd.CommandText = "SELECT OBJECT_ID(N'dbo.[Accounts]', N'U')";
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
        public IDbConnection GetConnection(string connectionString, ClaimsPrincipal arg)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }
        public void TestConnection(string connectionString)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            con.Dispose();
        }
    }
}