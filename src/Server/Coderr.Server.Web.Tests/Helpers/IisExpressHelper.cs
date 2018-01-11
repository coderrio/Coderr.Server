using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace codeRR.Server.Web.Tests.Helpers
{
    public class IisExpressHelper
    {
        private Process _process;

        /// <summary>
        ///     The application pool that IIS Express should use. Defaults to <c>"Clr4IntegratedAppPool"</c>.
        /// </summary>
        public string AppPool { get; set; }

        public string BaseUrl => "http://localhost:50473/coderr/";

        /// <summary>
        ///     Path to the IIS Express configuration file. Defaults to
        ///     <c>USER\DOCUMENTS\IISExpress\config\applicationhost.config</c>.
        /// </summary>
        public string ConfigPath { get; set; }

        public Dictionary<string, string> EnvironmentVariables { get; set; }

        /// <summary>
        ///     The path to the <c>iisexpress.exe</c> executable. Defaults to the default installation path
        ///     (e.g. <c>C:\Program Files (x86)\IIS Express\iisexpress.exe</c>), if not explicitly set.
        /// </summary>
        public string ExePath { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the associated IIS Express instance is currently running.
        /// </summary>
        public bool IsRunning => _process != null;

        /// <summary>
        ///     Starts IIS Express with the specified website.
        /// </summary>
        /// <param name="site">The website that IIS Express should host.</param>
        /// <exception cref="ArgumentNullException">
        ///     Argument <paramref name="site" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     There already is an associated IIS Express instance running (i.e. <see cref="IsRunning" /> is true).
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Either <see cref="ExePath" /> or <see cref="ConfigPath" /> could not be found.
        /// </exception>
        public void Start(string site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            if (_process != null)
                throw new InvalidOperationException("IIS Express is already running.");

            SetDefaultsWhereNecessary();

            if (!File.Exists(ExePath))
                throw new FileNotFoundException($"Path to IIS Express executable is invalid: '{ExePath}'.");

            if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                throw new FileNotFoundException($"Path to IIS Express configuration file is invalid: '{ConfigPath}'.");

            var iisExpressThread = new Thread(() => StartIisExpress(p => _process = p, site)) {IsBackground = true};
            iisExpressThread.Start();
            //StartIisExpress(p => _process = p, site);

            var attemptsLeft = 50;
            while (attemptsLeft > 0 && _process == null)
            {
                Thread.Sleep(200);
                attemptsLeft--;
            }

            if (_process == null)
                throw new InvalidOperationException("Failed to start IIS express");
        }

        /// <summary>
        ///     Stops the IIS Express instance that was formerly started via the <see cref="Start" /> method.
        /// </summary>
        public void Stop()
        {
            if (_process != null)
            {
                _process.Kill();
                _process.WaitForExit();
                _process.Dispose();
                _process = null;
            }
        }

        private void SetDefaultsWhereNecessary()
        {
            if (string.IsNullOrEmpty(ExePath))
                ExePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"IIS Express\iisexpress.exe");

            if (string.IsNullOrEmpty(ConfigPath))
                ConfigPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    @"IISExpress\config\applicationhost.config");

            if (string.IsNullOrEmpty(AppPool))
                AppPool = "Clr4IntegratedAppPool";
        }

        private void StartIisExpress(Action<Process> action, string site)
        {
            var isRunning = false;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExePath,
                    Arguments = $"/config:\"{Path.GetFullPath(ConfigPath)}\" /site:\"{site}\" /apppool:\"{AppPool}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            if (EnvironmentVariables != null)
            {
                foreach (var environmentVariable in EnvironmentVariables)
                {
                    Console.WriteLine($"Setting environment variable '{environmentVariable.Key}' to '{environmentVariable.Value}'");

                    process.StartInfo.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
                }
            }

            process.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);

                if (string.IsNullOrEmpty(args.Data))
                    return;

                if (args.Data.Contains("IIS Express is running"))
                    isRunning = true;
            };

            process.Start();
            process.BeginOutputReadLine();
            
            var attemptsLeft = 50;
            while (attemptsLeft > 0 && !isRunning)
            {
                Thread.Sleep(200);
                attemptsLeft--;
            }
            if (!isRunning)
                throw new InvalidOperationException("Failed to receive data from IIS ex");
           
            action(process);
        }
    }
}