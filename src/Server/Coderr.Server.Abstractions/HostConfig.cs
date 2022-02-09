using System;
using System.Diagnostics;

namespace Coderr.Server.Abstractions
{
    /// <summary>
    ///     Wraps either the configuration file or the Docker environment variables.
    /// </summary>
    public class HostConfig
    {
        public static HostConfig Instance = new HostConfig();

        private static bool _isTriggered;

        public bool IsRunningInDocker { get; set; }
        public string ConnectionString { get; set; }

        public bool IsDemo { get; set; } = Debugger.IsAttached;
        public bool IsConfigured { get; private set; }

        /// <summary>
        ///     Just to make it harder to take over an ongoing installation of Coderr on a public ip. Changed now and then.
        /// </summary>
        public string InstallationPassword { get; set; }

        public event EventHandler Configured;

        public override string ToString()
        {
            var tmp = IsRunningInDocker ? "[DOCKER] " : "[NATIVE] ";
            return $"{tmp} running with {ConnectionString}";
        }

        /// <summary>
        ///     Mark configuration as complete.
        /// </summary>
        public void MarkAsConfigured()
        {
            IsConfigured = true;
        }

        /// <summary>
        ///     Separate method since it must be run after everyone have subscribed on the event.
        /// </summary>
        public void TriggeredConfigured()
        {
            if (_isTriggered) return;

            _isTriggered = true;
            IsConfigured = true;
            Configured?.Invoke(this, EventArgs.Empty);
        }
    }
}