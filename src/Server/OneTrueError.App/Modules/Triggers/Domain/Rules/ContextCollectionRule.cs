using System;
using System.Linq;

namespace OneTrueError.App.Modules.Triggers.Domain.Rules
{
    /// <summary>
    /// Check a context collection in the trigger
    /// </summary>
    public class ContextCollectionRule : RuleBase, ITriggerRule
    {
        /// <summary>
        /// Context collection to check
        /// </summary>
        public string ContextName { get; set; }


        /// <summary>
        /// Property in that collection
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        /// Value for the property
        /// </summary>
        public string PropertyValue { get; set; }


        /// <summary>
        /// Validate report
        /// </summary>
        /// <param name="context">Context info</param>
        /// <returns>Recommendation</returns>
        public FilterResult Validate(FilterContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(ContextName))
            {
                foreach (var ctx in context.ErrorReport.ContextCollections)
                {
                    if (ctx.Properties.Any(property => Matches(PropertyValue, property.Value)))
                    {
                        return ResultToUse;
                    }
                }

                return FilterResult.NotMatched;
            }

            var errContext =
                context.ErrorReport.ContextCollections.FirstOrDefault(
                    x => x.Name.Equals(ContextName, StringComparison.CurrentCultureIgnoreCase));
            if (errContext == null)
                return FilterResult.NotMatched;

            var prop = errContext.Properties.Where(
                x => x.Key.Equals(PropertyName, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Value)
                .FirstOrDefault();

            if (prop == null)
                return FilterResult.NotMatched;

            return Matches(PropertyValue, prop) ? ResultToUse : FilterResult.NotMatched;
        }
    }
}