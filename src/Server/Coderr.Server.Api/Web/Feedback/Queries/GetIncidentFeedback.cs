using System;
using DotNetCqs;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Lists all feedback which has been made for an incident
    /// </summary>
    /// <remarks>
    ///     <para>Will only fetch for the most specific id</para>
    /// </remarks>
    [Message]
    public class GetIncidentFeedback : Query<GetIncidentFeedbackResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetIncidentFeedback" />.
        /// </summary>
        /// <param name="incidentId">Incident to get feedback for</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetIncidentFeedback(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     Incident to get feedback for
        /// </summary>
        public int IncidentId { get; private set; }
    }
}