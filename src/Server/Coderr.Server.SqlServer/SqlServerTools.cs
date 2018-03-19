using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Coderr.Server.Infrastructure;

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
        private readonly IConnectionFactory _connectionFactory;
        private readonly SchemaManager _schemaManager;

        public SqlServerTools(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _schemaManager = new SchemaManager(_connectionFactory.OpenConnection);
        }

        internal static string DbName { get; set; }

        private bool IsConnectionConfigured => _connectionFactory.IsConfigured;

        /// <summary>
        ///     Checks if the tables exists and are for the current DB schema.
        /// </summary>
        public bool GotUpToDateTables()
        {
            if (!IsConnectionConfigured)
                return false;

            using (var con = _connectionFactory.OpenConnection())
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

        /// <summary>
        ///     Check if the current DB schema is out of date compared to the embedded schema resources.
        /// </summary>
        public bool CanSchemaBeUpgraded()
        {
            return _schemaManager.CanSchemaBeUpgraded();
        }

        /// <summary>
        ///     Update DB schema to latest version.
        /// </summary>
        public void UpgradeDatabaseSchema()
        {
            _schemaManager.UpgradeDatabaseSchema();
        }
        
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Installation import = control over SQL")]
        public void CreateTables()
        {
            _schemaManager.CreateInitialStructure();
        }

        IDbConnection ISetupDatabaseTools.OpenConnection()
        {
            return _connectionFactory.OpenConnection();
        }

        
    }

    public interface IConnectionFactory
    {
        IDbConnection OpenConnection();
        bool IsConfigured { get; set; }
    }
}