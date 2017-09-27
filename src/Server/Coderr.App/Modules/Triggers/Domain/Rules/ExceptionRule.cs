using System;

namespace codeRR.Server.App.Modules.Triggers.Domain.Rules
{
    /// <summary>
    ///     Uses exception details (like Name, Message, StackTrace) to filter the trigger.
    /// </summary>
    public class ExceptionRule : RuleBase, ITriggerRule
    {
        /// <summary>
        ///     Exception field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///     Value to compare with
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Validate report
        /// </summary>
        /// <param name="context">Context info</param>
        /// <returns>Recommendation</returns>
        public FilterResult Validate(FilterContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.ErrorReport.Exception == null)
                return FilterResult.NotMatched;

            bool matches;
            switch (FieldName)
            {
                case "Exception.Name":
                    matches = Matches(Value, context.ErrorReport.Exception.Name);
                    break;
                case "Exception.Namespace":
                    matches = Matches(Value, context.ErrorReport.Exception.Namespace);
                    break;
                case "Exception.Assembly":
                    matches = Matches(Value, context.ErrorReport.Exception.AssemblyName);
                    break;
                case "Exception.StackTrace":
                    matches = Matches(Value, context.ErrorReport.Exception.StackTrace);
                    break;
                default:
                    throw new NotSupportedException(FieldName);
            }


            return matches ? ResultToUse : FilterResult.NotMatched;
        }
    }
}