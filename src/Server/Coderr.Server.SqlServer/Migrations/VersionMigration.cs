using System;
using System.Collections.Generic;

namespace Coderr.Server.SqlServer.Migrations
{
    public class VersionMigration
    {
        private readonly List<string> _scriptNames = new List<string>();

        public VersionMigration(int version)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));

            Version = version;
        }

        public string[] ScriptsNames => _scriptNames.ToArray();

        public int Version { get; private set; }

        public void Add(string scriptName)
        {
            if (scriptName == null) throw new ArgumentNullException(nameof(scriptName));

            if (scriptName.StartsWith("Update") && _scriptNames.Count> 0)
                _scriptNames.Insert(0, scriptName);
            else
                _scriptNames.Add(scriptName);
        }
    }
}