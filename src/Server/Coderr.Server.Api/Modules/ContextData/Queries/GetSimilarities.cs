using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.ContextData.Queries
{
    /// <summary>
    ///     Get similarities (i.e. analyzed context collections where we have normalized values and checked which values are
    ///     more frequently occurring).
    /// </summary>
    [Message]
    public class GetSimilarities : Query<GetSimilaritiesResult>
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetSimilarities()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetSimilarities" />.
        /// </summary>
        /// <param name="incidentId">incident to get similarities for</param>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        public GetSimilarities(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
        }

        /// <summary>
        ///     incident to get similarities for
        /// </summary>
        public int IncidentId { get; private set; }
    }
}