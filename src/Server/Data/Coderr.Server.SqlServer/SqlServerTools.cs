using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Coderr.Client;
using Coderr.Server.Infrastructure;
using Coderr.Server.SqlServer.Migrations;

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
        private static readonly List<MigrationRunner> _runners = new List<MigrationRunner>();

        public SqlServerTools(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public static void AddMigrationRunner(MigrationRunner runner)
        {
            if (runner == null) throw new ArgumentNullException(nameof(runner));
            if (_runners.Any(x => x.MigrationName == runner.MigrationName))
            {
                return;
            }

            _runners.Add(runner);
        }

        private bool IsTablesInstalled(IDbConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT OBJECT_ID(N'dbo.[Accounts]', N'U')";
                var result = cmd.ExecuteScalar();

                //null for SQL Express and DbNull for SQL Server
                return result != null && !(result is DBNull);
            }
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
                    return IsTablesInstalled(con);
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
            foreach (var runner in _runners)
            {
                runner.Run();
            }
        }

        IDbConnection ISetupDatabaseTools.OpenConnection()
        {
            return _connectionFactory();
        }

        private const string ConnectionTimeout = "Connection Timeout";
        private const string ConnectTimeout = "Connect Timeout";

        public void TestConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            connectionString = ChangeConnectionTimeout(connectionString);

            var con = new SqlConnection(connectionString);
            con.Open();
            con.Dispose();
        }

        private static string ChangeConnectionTimeout(string connectionString)
        {
            // both are valid.
            var pos = connectionString.IndexOf(ConnectionTimeout, StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
                pos = connectionString.IndexOf(ConnectTimeout, StringComparison.OrdinalIgnoreCase);

            if (pos != -1)
            {
                var valueStart =
                    connectionString.IndexOf("=", pos, StringComparison.OrdinalIgnoreCase);
                var valueEnd = connectionString.IndexOf(';', valueStart);
                connectionString = connectionString.Substring(0, valueStart) + "=5";
                if (valueEnd != -1 && valueEnd < connectionString.Length)
                {
                    connectionString = connectionString.Substring(0, valueStart) + "=5" +
                                       connectionString.Substring(valueEnd);
                }
                else
                {
                    connectionString = connectionString.Substring(0, valueStart) + "=5";
                }
            }
            else
            {
                connectionString = connectionString.TrimEnd(';') + ";" + ConnectionTimeout + "=5;";
            }

            return connectionString;
        }


        public void MarkConfigurationAsComplete()
        {
            using (var con = _connectionFactory())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "DELETE FROM Settings WHERE Section='Core' AND Name='Installation'";
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO Settings (Section, Name, Value) VALUES('Core', 'Installation', 'Complete')";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsConfigurationComplete(string connectionString)
        {
            try
            {
                TestConnection(connectionString);
            }
            catch
            {
                return false;
            }

            try
            {
                using (var con = _connectionFactory())
                {
                    if (!IsTablesInstalled(con))
                        return false;

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText =
                            "SELECT Value FROM Settings WHERE Section='Core' AND Name='Installation'";
                        var value = cmd.ExecuteScalar();
                        return value?.Equals("Complete") == true;
                    }
                }
            }
            catch (Exception ex)
            {
                Err.Report(ex, connectionString);
            }

            return false;
        }
    }
}