using System;
using System.Collections.Generic;
using Coderr.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.Abstractions.Incidents
{
    public class SolutionProviderContext
    {
        private readonly List<SuggestedIncidentSolution> _possibleSolutions;

        public SolutionProviderContext(List<SuggestedIncidentSolution> possibleSolutions)
        {
            _possibleSolutions = possibleSolutions;
        }

        public int ApplicationId { get; set; }
        public string Description { get; set; }

        /// <summary>
        ///     Namespace + name of exception
        /// </summary>
        public string FullName { get; set; }

        public int IncidentId { get; set; }

        public string StackTrace { get; set; }
        public string[] Tags { get; set; }

        public void AddSuggestion(string suggestion, string motivation)
        {
            if (suggestion == null) throw new ArgumentNullException(nameof(suggestion));
            if (motivation == null) throw new ArgumentNullException(nameof(motivation));
            _possibleSolutions.Add(new SuggestedIncidentSolution {Reason = motivation, SuggestedSolution = suggestion});
        }
    }
}