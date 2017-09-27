using System;
using System.Diagnostics;

namespace codeRR.Server.App.Modules.Similarities.Domain
{
    /// <summary>
    ///     Holds the similarity value, its percentage and total count of similarities
    /// </summary>
    public class SimilarityValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimilarityValue" /> class.
        /// </summary>
        /// <param name="value">Normalized value.</param>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public SimilarityValue(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Value = value;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SimilarityValue" />.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="percentage">percentage, 0-100</param>
        /// <param name="count">number of times we've got this value</param>
        public SimilarityValue(string value, int percentage, int count)
        {
            Value = value;
            Count = count;
            Percentage = percentage;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected SimilarityValue()
        {
        }

        /// <summary>
        ///     Number of times that this item has been added.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Percentage, 1-100.
        /// </summary>
        public int Percentage { get; private set; }

        /// <summary>
        ///     Normalized value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        ///     Increase usage of this value
        /// </summary>
        /// <param name="totalCount">Total count for all values (not just this one)</param>
        public void IncreaseUsage(int totalCount)
        {
            if (totalCount <= 0) throw new ArgumentOutOfRangeException("totalCount");

            Count += 1;
            Percentage = Count*100/totalCount;
            if (Percentage > 100)
                Debugger.Break();
        }

        /// <summary>
        ///     Each time a new value is added for a similarity we have to recalculate the others.
        /// </summary>
        /// <param name="totalCount"></param>
        public void Recalculate(int totalCount)
        {
            if (totalCount <= 0) throw new ArgumentOutOfRangeException("totalCount");

            Percentage = Count*100/totalCount;
        }
    }
}