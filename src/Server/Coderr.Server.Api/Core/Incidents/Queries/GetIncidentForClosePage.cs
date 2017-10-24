using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Get incident information tailored for the close page.
    /// </summary>
    [Message]
    public class GetIncidentForClosePage : Query<GetIncidentForClosePageResult>
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetIncidentForClosePage()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetIncidentForClosePage" />.
        /// </summary>
        /// <param name="incidentId">incident id</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetIncidentForClosePage(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Incident id
        /// </summary>
        public int IncidentId { get; private set; }
    }
}