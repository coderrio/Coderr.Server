using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Events
{
    /// <summary>
    ///     Our user have configured that all new reports for this incident should be ignored
    /// </summary>
    [Message]
    public class IncidentIgnored
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentIgnored" />.
        /// </summary>
        /// <param name="incidentId">incident being ignored</param>
        /// <param name="accountId">account ignoring the incident</param>
        /// <param name="userName">userName for the given account</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IncidentIgnored(int incidentId, int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            IncidentId = incidentId;
            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected IncidentIgnored()
        {
        }

        /// <summary>
        ///     User that configured ignore.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Name of the user.
        /// </summary>
        public string UserName { get; set; }
    }
}