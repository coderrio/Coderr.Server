using System;

namespace codeRR.Server.Api.Modules.ContextData.Queries
{
    /// <summary>
    ///     A property in <see cref="GetSimilaritiesCollection" />.
    /// </summary>
    public class GetSimilaritiesSimilarity
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetSimilaritiesSimilarity" />.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public GetSimilaritiesSimilarity(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Values = new GetSimilaritiesValue[0];
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetSimilaritiesSimilarity()
        {
            Values = new GetSimilaritiesValue[0];
        }

        /// <summary>
        ///     Name of this similarity.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        ///     The different values that this one have got.
        /// </summary>
        public GetSimilaritiesValue[] Values { get; set; }
    }
}