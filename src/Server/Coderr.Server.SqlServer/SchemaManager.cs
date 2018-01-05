using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace codeRR.Server.SqlServer
{
    public class SchemaManager
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private const string SchemaNamespace = "codeRR.Server.SqlServer.Schema";
        private ILog _logger = LogManager.GetLogger(typeof(SchemaManager));

        public SchemaManager(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Check if the current DB schema is out of date compared to the embedded schema resources.
        /// </summary>
        public bool CanSchemaBeUpgraded()
        {
            var version = GetCurrentSchemaVersion();
            var embeddedSchema = GetLatestSchemaVersion();
            return embeddedSchema > version;
        }

        private bool invoked = false;

        public void CreateInitialStructure()
        {
            if (invoked)
                throw new InvalidOperationException("Invoked");
            invoked = true;
            using (var con = _connectionFactory())
            {
                var resourceName = $"{SchemaNamespace}.Database.sql";
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                var sql = new StreamReader(res).ReadToEnd();
                using (var transaction = con.BeginTransaction())
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    Console.WriteLine("CON " + con.ConnectionString + " SQL: " + sql);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public int GetCurrentSchemaVersion()
        {
            var version = 0;
            using (var con = _connectionFactory())
            {
                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        var sql = "SELECT Version FROM DatabaseSchema";
                        cmd.CommandText = sql;
                        var result = cmd.ExecuteScalar();
                        version = (int)result;
                    }
                    catch (SqlException ex)
                    {
                        //invalid object name
                        if (ex.Number == 208)
                            return -1;

                        throw;
                    }
                }
            }
            return version;
        }

        public int GetLatestSchemaVersion()
        {
            var highestVersion = 0;
            var names =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames()
                    .Where(x => x.StartsWith(SchemaNamespace) && x.Contains(".Update."));
            foreach (var name in names)
            {
                var pos = name.IndexOf("Update") + 8; //2 extra for ".v"
                var endPos = name.IndexOf(".", pos);
                var versionStr = name.Substring(pos, endPos - pos);
                var version = int.Parse(versionStr);
                if (version > highestVersion)
                    highestVersion = version;
            }
            return highestVersion;
        }

        /// <summary>
        /// Upgrade schema
        /// </summary>
        /// <param name="toVersion">-1 = latest version</param>
        public void UpgradeDatabaseSchema(int toVersion = -1)
        {
            if (toVersion == -1)
                toVersion = GetLatestSchemaVersion();
            var currentSchema = GetCurrentSchemaVersion();
            if (currentSchema < 1)
                currentSchema = 1;

            for (var version = currentSchema + 1; version <= toVersion; version++)
            {
                var schema = GetSchema(version);
                using (var con = _connectionFactory())
                {
                    using (var transaction = con.BeginTransaction())
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = schema;
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }

        private string GetSchema(int version)
        {
            var resourceName = $"{SchemaNamespace}.Update.v{version}.sql";
            var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (res == null)
                throw new InvalidOperationException("Failed to find schema " + resourceName);

            return new StreamReader(res).ReadToEnd();
        }
    }
}