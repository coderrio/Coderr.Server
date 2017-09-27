using System;
using System.Collections.Generic;

namespace codeRR.Server.Api.Modules.ContextData.Queries
{
    /// <summary>
    ///     Context collection for <see cref="GetSimilaritiesResult" />.
    /// </summary>
    public class GetSimilaritiesCollection
    {
        private List<GetSimilaritiesSimilarity> _similarities = new List<GetSimilaritiesSimilarity>();

        /// <summary>
        ///     Name of this collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     An analyzed property and all its values.
        /// </summary>
        public GetSimilaritiesSimilarity[] Similarities
        {
            get { return _similarities.ToArray(); }
            set { _similarities = new List<GetSimilaritiesSimilarity>(value); }
        }

        /// <summary>
        ///     Add an analyzed property.
        /// </summary>
        /// <param name="similarity">property + values</param>
        /// <exception cref="ArgumentNullException">similarity</exception>
        public void Add(GetSimilaritiesSimilarity similarity)
        {
            if (similarity == null) throw new ArgumentNullException("similarity");
            _similarities.Add(similarity);
        }
    }
}