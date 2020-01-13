using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Coderr.Server.SqlServer.Migrations
{
    public class MigrationScripts
    {
        private readonly string _migrationName;
        private readonly Dictionary<int, string> _versions = new Dictionary<int, string>();
        private Assembly _scriptAssembly;
        public MigrationScripts(string migrationName, Assembly scriptAssembly)
        {
            _migrationName = migrationName ?? throw new ArgumentNullException(nameof(migrationName));
            _scriptAssembly = scriptAssembly ?? throw new ArgumentNullException(nameof(scriptAssembly));
        }

        public MigrationScripts(string migrationName)
        {
            _migrationName = migrationName ?? throw new ArgumentNullException(nameof(migrationName));
        }

        public bool IsEmpty => _versions.Count == 0;

        public void AddScriptName(int version, string scriptName)
        {
            if (version < 0) throw new ArgumentOutOfRangeException(nameof(version));

            _versions[version] = scriptName ?? throw new ArgumentNullException(nameof(scriptName));
        }

        public string LoadScript(int version)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));

            var scriptName = _versions[version];
            var res = _scriptAssembly.GetManifestResourceStream(scriptName);
            if (res == null)
                throw new InvalidOperationException("Failed to find schema " + scriptName);

            return new StreamReader(res).ReadToEnd();
        }

        public void Execute(IDbConnection connection, int version)
        {
            var script = LoadScript(version);
            var sb = new StringBuilder();
            var sr = new StringReader(script);
            while (true)
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;

                if (!line.Equals("go"))
                {
                    sb.AppendLine(line);
                    continue;
                }

                ExecuteSql(connection, sb.ToString());
                sb.Clear();

            }

            //do the remaining part of the script (or everything if GO was not used).
            ExecuteSql(connection, sb.ToString());

            ExecuteSql(connection, $"UPDATE DatabaseSchema SET Version={version} WHERE Name = '{_migrationName}'");
        }

        private static void ExecuteSql(IDbConnection connection, string sql)
        {
            var parts = sql.Split(new[] {"\r\ngo\r\n", "\r\nGO\r\n", "\r\ngo;\r\n"},
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = part;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public int GetHighestVersion()
        {
            if (_versions.Count == 0)
                return -1;
            return _versions.Max(x => x.Key);
        }
    }
}