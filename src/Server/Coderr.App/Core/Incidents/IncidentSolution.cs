using System;

namespace codeRR.Server.App.Core.Incidents
{
    /// <summary>
    ///     How the development team solved the incident.
    /// </summary>
    public class IncidentSolution
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IncidentSolution" />.
        /// </summary>
        /// <param name="createdBy">AccountId for the user that wrote the solution</param>
        /// <param name="description">Markdown formatted solution description</param>
        /// <exception cref="ArgumentNullException">createdBy</exception>
        /// <exception cref="ArgumentOutOfRangeException">description</exception>
        public IncidentSolution(int createdBy, string description)
        {
            if (description == null) throw new ArgumentNullException("description");
            if (createdBy <= 0) throw new ArgumentOutOfRangeException("createdBy");

            Description = description;
            CreatedBy = createdBy;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected IncidentSolution()
        {
        }

        /// <summary>
        ///     When this solution was created
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     AccountId for the user that wrote the solution
        /// </summary>
        public int CreatedBy { get; private set; }


        /// <summary>
        ///     Markdown formatted solution description
        /// </summary>
        public string Description { get; private set; }
    }
}