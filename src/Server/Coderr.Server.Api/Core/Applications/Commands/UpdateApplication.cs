using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Update application
    /// </summary>
    [Message]
    public class UpdateApplication
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UpdateApplication" />.
        /// </summary>
        /// <param name="applicationId">Application to update</param>
        /// <param name="name">New application name</param>
        public UpdateApplication(int applicationId, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            Name = name;
        }

        /// <summary>
        ///     Application to change
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     New application name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Update type of application
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used when analyzing context data
        ///     </para>
        /// </remarks>
        public TypeOfApplication? TypeOfApplication { get; set; }
    }
}