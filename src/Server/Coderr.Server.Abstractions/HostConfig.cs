using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Abstractions
{
    /// <summary>
    /// Wraps either the configuration file or the Docker environment variables.
    /// </summary>
    public class HostConfig
    {
        public static HostConfig Instance = new HostConfig();

        public bool IsRunningInDocker { get; set; }
        public string ConnectionString { get; set; }
        public bool IsConfigured { get; set; }
        public string ConfigurationPassword { get; set; }

    }
}
