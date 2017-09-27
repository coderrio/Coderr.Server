using System;

namespace codeRR.Server.App.Modules.Triggers.Domain.Rules
{
    /// <summary>
    ///     Base for trigger rules
    /// </summary>
    public class RuleBase
    {
        /// <summary>
        ///     How to compare the values
        /// </summary>
        public FilterCondition Condition { get; set; }

        /// <summary>
        ///     Result to use if value comparison succeeds.
        /// </summary>
        public FilterResult ResultToUse { get; set; }

        /// <summary>
        ///     Match
        /// </summary>
        /// <param name="value1">first value</param>
        /// <param name="value2">second value</param>
        /// <returns><c>true</c> if values matches according to <see cref="Condition" />; otherwise <c>false</c>.</returns>
        public bool Matches(string value1, string value2)
        {
            if (value1 == null) throw new ArgumentNullException("value1");

            switch (Condition)
            {
                case FilterCondition.Contains:
                    return value1.IndexOf(value2, StringComparison.CurrentCultureIgnoreCase) > -1;
                case FilterCondition.DoNotContain:
                    return value1.IndexOf(value2, StringComparison.CurrentCultureIgnoreCase) == -1;
                case FilterCondition.EndsWith:
                    return value1.EndsWith(value2, StringComparison.CurrentCultureIgnoreCase);
                case FilterCondition.StartsWith:
                    return value1.StartsWith(value2, StringComparison.CurrentCultureIgnoreCase);
                case FilterCondition.Equals:
                    return value1.Equals(value2, StringComparison.CurrentCultureIgnoreCase);
                default:
                    throw new NotSupportedException(Condition.ToString());
            }
        }
    }
}