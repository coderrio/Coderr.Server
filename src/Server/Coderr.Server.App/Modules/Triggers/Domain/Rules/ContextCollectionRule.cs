using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.App.Modules.Triggers.Domain.Rules
{
    /// <summary>
    ///     Check a context collection in the trigger
    /// </summary>
    public class ContextCollectionRule : RuleBase, ITriggerRule
    {
        /// <summary>
        ///     Context collection to check
        /// </summary>
        public string ContextName { get; set; }


        /// <summary>
        ///     Property in that collection
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        ///     Value for the property
        /// </summary>
        public string PropertyValue { get; set; }


        /// <summary>
        ///     Validate report
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
                    if (Enumerable.Any<KeyValuePair<string, string>>(ctx.Properties, property => Matches(PropertyValue, property.Value)))
                    {
                        return ResultToUse;
                    }
                }

                return FilterResult.NotMatched;
            }

            var errContext =
                Enumerable.FirstOrDefault(context.ErrorReport.ContextCollections, x => x.Name.Equals(ContextName, StringComparison.CurrentCultureIgnoreCase));
            if (errContext == null)
                return FilterResult.NotMatched;

            var prop = Enumerable.Where<KeyValuePair<string, string>>(errContext.Properties, x => x.Key.Equals(PropertyName, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Value)
                .FirstOrDefault();

            if (prop == null)
                return FilterResult.NotMatched;

            return Matches(PropertyValue, prop) ? ResultToUse : FilterResult.NotMatched;
        }
    }
}