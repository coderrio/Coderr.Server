using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.PostgreSQL.Migrations
{
    public class MigrationRunner
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly string _migrationName;
        private readonly string _scriptNamespace;
        private readonly MigrationScripts _scripts;
        private ILog _logger = LogManager.GetLogger(typeof(MigrationRunner));
        private Assembly _scriptAssembly;

        public MigrationRunner(Func<IDbConnection> connectionFactory, string migrationName, string scriptNamespace)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _migrationName = migrationName ?? throw new ArgumentNullException(nameof(migrationName));
            _scriptNamespace = scriptNamespace;
            _scriptAssembly = GetScriptAssembly(migrationName);
            _scripts = new MigrationScripts(migrationName, _scriptAssembly);
        }

        private Assembly GetScriptAssembly(string migrationName)
        {
            if (migrationName == null) throw new ArgumentNullException(nameof(migrationName));

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;
                if (assembly.GetName().Name?.StartsWith("Coderr", StringComparison.OrdinalIgnoreCase) != true)
                    continue;

                var isFound = assembly
                    .GetManifestResourceNames()
                    .Any(x => x.StartsWith(_scriptNamespace) && x.Contains($"{_migrationName}.v"));
                if (isFound)
                    return assembly;
            }
            throw new InvalidOperationException($"Failed to find scripts for migration '{migrationName}'.");
        }

        public void Run()
        {
            EnsureLoaded();
            if (!CanSchemaBeUpgraded())
            {
                _logger.Debug("Db Schema is up to date.");
                return;
            }

            _logger.Info("Updating DB schema.");
            UpgradeDatabaseSchema();
        }

        private void EnsureLoaded()
        {
            if (_scripts.IsEmpty)
                LoadScripts();
        }

        /// <summary>
        ///     Check if the current DB schema is out of date compared to the embedded schema resources.
        /// </summary>
        protected bool CanSchemaBeUpgraded()
        {
            var version = GetCurrentSchemaVersion();
            var embeddedSchema = GetLatestSchemaVersion();
            return embeddedSchema > version;
        }

        protected void LoadScripts()
        {
            var names =
                _scriptAssembly
                    .GetManifestResourceNames()
                    .Where(x => x.StartsWith(_scriptNamespace) && x.Contains($"{_migrationName}.v"));

            foreach (var name in names)
            {
                var pos = name.IndexOf(".v") + 2; //2 extra for ".v"
                var endPos = name.IndexOf(".", pos);
                var versionStr = name.Substring(pos, endPos - pos);
                var version = int.Parse(versionStr);
                _scripts.AddScriptName(version, name);
            }
        }

        public int GetCurrentSchemaVersion()
        {
            string[] scripts = new[]
            {
                @"IF OBJECT_ID (N'DatabaseSchema', N'U') IS NULL
                        BEGIN
                            CREATE TABLE [dbo].DatabaseSchema (
                                [Version] int not null default 1,
                                [Name] varchar(50) NOT NULL
                            );
                        END",

                @"IF COL_LENGTH('DatabaseSchema', 'Name') IS NULL
                    BEGIN
                        ALTER TABLE DatabaseSchema ADD [Name] varchar(50) NULL;
                    END;",
                @"UPDATE DatabaseSchema SET Name = 'coderr' WHERE Name IS NULL"
            };

            using (var con = _connectionFactory())
            {
                foreach (var script in scripts)
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }

                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT Version FROM DatabaseSchema WHERE Name = @name;";
                        cmd.AddParameter("name", _migrationName);
                        var result = cmd.ExecuteScalar();
                        if (result is null)
                            return -1;
                        return (int)result;
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
        }

        public int GetLatestSchemaVersion()
        {
            EnsureLoaded();
            return _scripts.GetHighestVersion();
        }

        /// <summary>
        ///     Upgrade schema
        /// </summary>
        /// <param name="toVersion">-1 = latest version</param>
        protected void UpgradeDatabaseSchema(int toVersion = -1)
        {
            if (toVersion == -1)
                toVersion = GetLatestSchemaVersion();
            var currentSchema = GetCurrentSchemaVersion();
            if (currentSchema < 1)
                currentSchema = 0;

            for (var version = currentSchema + 1; version <= toVersion; version++)
            {
                _logger.Info("Migrating to v" + version);
                using (var con = _connectionFactory())
                {
                    _scripts.Execute(con, version);
                    if (version == 1)
                    {
                        con.ExecuteNonQuery($"INSERT INTO DatabaseSchema (Name, Version) VALUES('{_migrationName}', 1);");
                    }
                }
            }
        }
    }
}