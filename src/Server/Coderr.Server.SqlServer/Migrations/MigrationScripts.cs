using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Coderr.Server.SqlServer.Migrations
{
    public class MigrationScripts
    {
        internal const string SchemaNamespace = "Coderr.Server.SqlServer.Schema";
        private readonly Dictionary<int, VersionMigration> _versions = new Dictionary<int, VersionMigration>();
        private bool _isEmpty;

        public bool IsEmpty => _versions.Count == 0;

        public void AddScript(int version, string scriptName)
        {
            if (scriptName == null) throw new ArgumentNullException(nameof(scriptName));
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));

            if (!_versions.TryGetValue(version, out var value))
            {
                value = new VersionMigration(version);
                _versions[version] = value;
            }

            value.Add(scriptName);
        }

        public IEnumerable<string> GetScripts(int version)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));

            var scriptNames = _versions[version].ScriptsNames;
            foreach (var scriptName in scriptNames)
            {
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(scriptName);
                if (res == null)
                    throw new InvalidOperationException("Failed to find schema " + scriptName);

                yield return new StreamReader(res).ReadToEnd();
            }
        }

        public int GetHighestVersion()
        {
            return _versions.Max(x => x.Key);
        }
    }
}