using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace codeRR.Server.App.Modules.Similarities.Domain
{
    /// <summary>
    ///     Information about a specific context collection property with all if it's values.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         No support for <c>null</c> values internally, so they are converted into empty strings. Hence any similarity
    ///         with an empty string might have been <c>null</c> too.
    ///     </para>
    /// </remarks>
    public class Similarity
    {
        //do not set as readonly, destroys serialization
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IDictionary<string, SimilarityValue> _similarityValues;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Similarity" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     contextCollectionName
        ///     or
        ///     propertyName
        ///     or
        ///     value
        /// </exception>
        public Similarity(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            PropertyName = propertyName;
            ValueCount = 0;
            _similarityValues = new Dictionary<string, SimilarityValue>();
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Similarity()
        {
        }

        /// <summary>
        ///     Id of <see cref="SimilarityCollection" />.
        /// </summary>
        public int CollectionId { get; set; }

        /// <summary>
        ///     Similarity id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name of the property
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     Amount of added values (i.e. total number of times we've added a count to a value).
        /// </summary>
        public int ValueCount { get; set; }

        /// <summary>
        ///     All values which have been collected for this similarity (i.e. context collection property)
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public IEnumerable<SimilarityValue> Values
        {
            get { return _similarityValues.Values; }
            // ReSharper disable once UnusedMember.Local ==> used by serialization.
            private set { _similarityValues = value.ToDictionary(x => x.Value); }
        }

        /// <summary>
        ///     Adds a similarity value, if the similarity value exists its count is incremented.
        /// </summary>
        /// <param name="value">Value may be null</param>
        public void AddValue(string value)
        {
            if (value == null)
                value = "";

            SimilarityValue similarityValue;
            if (!_similarityValues.TryGetValue(value, out similarityValue))
            {
                similarityValue = new SimilarityValue(value);
                _similarityValues.Add(value, similarityValue);
            }

            ValueCount += 1;
            similarityValue.IncreaseUsage(ValueCount);

            foreach (var value1 in _similarityValues)
            {
                value1.Value.Recalculate(ValueCount);
            }
        }

        /// <summary>
        ///     Invoked during fetching.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddValue(SimilarityValue value)
        {
            if (value == null) throw new ArgumentNullException("value");

            _similarityValues.Add(value.Value, value);
        }

        /// <summary>
        ///     Get the value with highest percentage count.
        /// </summary>
        /// <returns>Value</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public SimilarityValue GetMostFrequentlyUsedValue()
        {
            return _similarityValues.Values
                .OrderByDescending(x => x.Percentage)
                .First();
        }

        /// <summary>
        ///     Invoked during fetch (i.e. repository load)
        /// </summary>
        /// <param name="values"></param>
        public void LoadValues(SimilarityValue[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            foreach (var value in values)
            {
                //TODO: Find out why this bug occur
                // i.e. sometimes the exact same value is repeated over multiple values.
                if (_similarityValues.ContainsKey(value.Value))
                    _similarityValues[value.Value].IncreaseUsage(ValueCount);
                else
                    _similarityValues.Add(value.Value, value);
            }
            if (ValueCount == 0)
                ValueCount = _similarityValues.Sum(x => x.Value.Count);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", PropertyName, string.Join(", ", Values.Select(x => x.Value)));
        }
    }
}