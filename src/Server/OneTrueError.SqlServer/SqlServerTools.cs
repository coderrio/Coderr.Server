using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using OneTrueError.Infrastructure;

namespace OneTrueError.SqlServer
{
    public class SqlServerTools : ISetupDatabaseTools
    {
        private bool IsConnectionConfigured
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Db"] != null &&
                       !string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings["Db"].ConnectionString);
            }
        }

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

            var con = new SqlConnection(connectionString);
            con.Open();
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Installation import = control over SQL")]
        public void CreateTables()
        {
            using (var con = OpenConnection())
            {
                var resourceName = GetType().Namespace + ".Database.sql";
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                var sql = new StreamReader(res).ReadToEnd();
                using (var transaction = con.BeginTransaction())
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        IDbConnection ISetupDatabaseTools.OpenConnection()
        {
            return OpenConnection();
        }

        public static IDbConnection OpenConnection(string connectionString)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }

        public static IDbConnection OpenConnection()
        {
            var conStr = ConfigurationManager.ConnectionStrings["Db"];
            var provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            var connection = provider.CreateConnection();
            connection.ConnectionString = conStr.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}