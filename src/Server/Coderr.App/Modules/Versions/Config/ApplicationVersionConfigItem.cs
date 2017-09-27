using System;

namespace codeRR.Server.App.Modules.Versions.Config
{
    /// <summary>
    /// Configuration for a specific application
    /// </summary>
    /// <seealso cref="ApplicationVersionConfig"/>
    public class ApplicationVersionConfigItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationVersionConfigItem"/>.
        /// </summary>
        /// <param name="applicationId">application that an assembly name is defined for</param>
        /// <param name="assemblyName">the application to use to extract version</param>
        /// <exception cref="ArgumentNullException">assemblyName</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public ApplicationVersionConfigItem(int applicationId, string assemblyName)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Application that this version assembly have been configured for.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        /// Assembly to extract version from
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Change to a new assembly name
        /// </summary>
        /// <param name="assemblyName">Name</param>
        public void ChangeAssembly(string assemblyName)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            AssemblyName = assemblyName;
        }
    }
}