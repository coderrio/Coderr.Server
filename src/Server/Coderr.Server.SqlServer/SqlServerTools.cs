using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using codeRR.Server.Infrastructure;
using codeRR.Server.SqlServer.Tools;

namespace codeRR.Server.SqlServer
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
        private readonly SchemaManager _schemaManager;

        public SqlServerTools()
        {
            _schemaManager = new SchemaManager(OpenConnection);
        }

        internal static string DbName { get; set; }

        private bool IsConnectionConfigured
        {
            get
            {
                var conString = GetConnectionString(false);
                return !string.IsNullOrEmpty(conString?.ConnectionString);
            }
        }

        /// <summary>
        ///     Checks if the tables exists and are for the current DB schema.
        /// </summary>
        public bool GotUpToDateTables()
        {
            if (!IsConnectionConfigured)
                return false;

            using (var con = OpenConnection())
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

        public void CheckConnectionString(string connectionString)
        {
            var pos = connectionString.IndexOf("Connect Timeout=");
            if (pos != -1)
            {
                pos += "Connect Timeout=".Length;
                var endPos = connectionString.IndexOf(";", pos);
                if (endPos == -1)
                    connectionString = connectionString.Substring(0, pos) + "1";
                else
                    connectionString = connectionString.Substring(0, pos) + "1" + connectionString.Substring(endPos);
            }
            SqlConnection.ClearAllPools();
            var con = new SqlConnection(connectionString);
            con.Open();
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Installation import = control over SQL")]
        public void CreateTables()
        {
            _schemaManager.CreateInitialStructure();
        }

        IDbConnection ISetupDatabaseTools.OpenConnection()
        {
            return OpenConnection();
        }

        public static IDbConnection OpenConnection(string connectionString)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            if (DbName != null)
                con.ChangeDatabase(DbName);

            return con;
        }

        protected ConnectionStringSettings GetConnectionString(bool throwOnError = true)
        {
            var db = ConfigurationManager.ConnectionStrings["Db"];
            if (db == null && throwOnError)
                throw new ConfigurationErrorsException("The connectionString 'Db' is missing in web.config.");

            return db;
        }

        private IDbConnection OpenConnection()
        {
            var conStr = GetConnectionString();
            var provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            var connection = provider.CreateConnection();
            connection.ConnectionString = conStr.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}