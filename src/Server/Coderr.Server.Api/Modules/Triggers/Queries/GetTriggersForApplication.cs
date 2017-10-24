using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Triggers.Queries
{
    /// <summary>
    ///     Get all triggers for an application
    /// </summary>
    [Message]
    public class GetTriggersForApplication : Query<TriggerDTO[]>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetTriggersForApplication" />.
        /// </summary>
        /// <param name="applicationId">application to get triggers for</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public GetTriggersForApplication(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetTriggersForApplication()
        {
        }

        /// <summary>
        ///     Application
        /// </summary>
        public int ApplicationId { get; set; }
    }
}