using System;

namespace codeRR.Server.Api.Modules.ContextData.Queries
{
    /// <summary>
    ///     A single value for <see cref="GetSimilaritiesSimilarity" />.
    /// </summary>
    public class GetSimilaritiesValue
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetSimilaritiesValue" />.
        /// </summary>
        /// <param name="value">Value, null is allowed</param>
        /// <param name="percentage">0-100</param>
        /// <param name="count">Number of times that this value have been received.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GetSimilaritiesValue(string value, int percentage, int count)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException("percentage", percentage,
                    "Percentage should be between 0 and 100.");
            if (count <= 0) throw new ArgumentOutOfRangeException("count");

            Value = value;
            Percentage = percentage;
            Count = count;
        }

        /// <summary>
        ///     Number of times that this value have been found in an error report.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     0-100
        /// </summary>
        public int Percentage { get; set; }

        /// <summary>
        ///     Value for this item
        /// </summary>
        public string Value { get; set; }
    }
}