using System;
using Coderr.Server.Api.Insights.Queries;

namespace Coderr.Server.App.Insights.Keyfigures
{
    public class KeyPerformanceIndicator
    {
        private ValueIdType? _toplistValueType;

        /// <summary>
        /// </summary>
        /// <param name="name">
        ///     Technical name, used to be able to store values and compare them later on. SHould be constant once
        ///     defined (while the title can be changed)
        /// </param>
        /// <param name="title">Display name</param>
        /// <param name="valueComparison">How to calculate the scoring used to compare applications</param>
        public KeyPerformanceIndicator(string name, string title, IndicatorValueComparison valueComparison)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            ComparisonType = valueComparison;
        }

        public int? PeriodValueId { get; set; }

        /// <summary>
        ///     Technical name, used to be able to store values and compare them later on. SHould be constant once defined (while
        ///     the title can be changed)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Not all applications can be included when the client has many. In those cases we generate worst/best variations of an KPI. 
        /// </summary>
        public bool IsVariation { get; set; }

        public string Title { get; private set; }

        /// <summary>
        ///     Explains what the KPI represents
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Comment (for instance if there is a runner up or if it's higher or lower than the global average)
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        ///     Value for the last 30 days
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Show the application name as value (if this property is specified).
        /// </summary>
        public int? ValueId { get; set; }

        public ValueIdType ValueIdType { get; set; }

        /// <summary>
        ///     Value for the last 90 days
        /// </summary>
        public object PeriodValue { get; set; }

        /// <summary>
        ///     Same amount of values as <see cref="KeyPerformanceIndicatorContext.TrendDates" />.
        /// </summary>
        /// <value>
        ///     Must be a number format (decimal or int)
        /// </value>
        public TrendLine[] TrendLines { get; set; }

        /// <summary>
        ///     How do we compare values with other
        /// </summary>
        public IndicatorValueComparison ComparisonType { get; private set; }

        /// <summary>
        ///     When we need to tell what kind of value it is (like "days")
        /// </summary>
        public string ValueUnit { get; set; }

        public KeyPerformanceIndicatorTopListItem[] Toplist { get; set; }

        public ValueIdType ToplistValueType
        {
            get => _toplistValueType ?? ValueIdType;
            set => _toplistValueType = value;
        }

        /// <summary>
        /// This indicator represents a value which is for all developers, and should be normalized when comparing to other applications.
        /// </summary>
        public bool CanBeNormalized { get; set; }

        public override string ToString()
        {
            return $"{Name} {Value}{ValueUnit}{ValueId} {PeriodValue}{PeriodValueId}";
        }
    }

    public enum ValueIdType
    {
        None,
        AccountId,
        ApplicationId
    }
}