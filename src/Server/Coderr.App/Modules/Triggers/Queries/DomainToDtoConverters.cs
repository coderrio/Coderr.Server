using System;
using System.Diagnostics.CodeAnalysis;
using codeRR.Server.Api.Modules.Triggers;
using codeRR.Server.App.Modules.Triggers.Domain;
using codeRR.Server.App.Modules.Triggers.Domain.Rules;

namespace codeRR.Server.App.Modules.Triggers.Queries
{
    /// <summary>
    ///     Converts triggers into DTOs which are transferred to the UI
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Unknown enum values and unknown sub classes should generate exceptions (<c>FormatException</c>) so that we know
    ///         that the converts have not been updated.
    ///     </para>
    /// </remarks>
    public static class DomainToDtoConverters
    {
        /// <summary>
        ///     Convert action to a DTO
        /// </summary>
        /// <param name="action">entity</param>
        /// <returns>DTO</returns>
        public static TriggerActionDataDTO ConvertAction(ActionConfigurationData action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return new TriggerActionDataDTO
            {
                ActionContext = action.Data,
                ActionName = action.ActionName
            };
        }

        /// <summary>
        ///     Convert entity to DTO
        /// </summary>
        /// <param name="filter">entity</param>
        /// <returns>DTO</returns>
        /// <exception cref="FormatException">Unknown enum value in entity</exception>
        public static TriggerFilterCondition ConvertFilterCondition(FilterCondition filter)
        {
            switch (filter)
            {
                case FilterCondition.Contains:
                    return TriggerFilterCondition.Contains;
                case FilterCondition.DoNotContain:
                    return TriggerFilterCondition.DoNotContain;
                case FilterCondition.EndsWith:
                    return TriggerFilterCondition.EndsWith;
                case FilterCondition.Equals:
                    return TriggerFilterCondition.Equals;
                case FilterCondition.StartsWith:
                    return TriggerFilterCondition.StartsWith;
                default:
                    throw new FormatException(
                        $"Value '{filter}' do not exist in the {typeof(TriggerFilterCondition).Name} enum.");
            }
        }


        /// <summary>
        ///     Convert filter
        /// </summary>
        /// <param name="ruleAction">entity</param>
        /// <returns>DTO</returns>
        /// <exception cref="FormatException">Entity enum contains a value that is currently not handled.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterResult")]
        public static TriggerRuleAction ConvertFilterResult(FilterResult ruleAction)
        {
            switch (ruleAction)
            {
                case FilterResult.Revoke:
                    return TriggerRuleAction.AbortTrigger;
                case FilterResult.Continue:
                    return TriggerRuleAction.ContinueWithNextRule;
                case FilterResult.Grant:
                    return TriggerRuleAction.ExecuteActions;
                default:
                    throw new FormatException(
                        $"Value '{ruleAction}' do not exist in the {typeof(TriggerRuleAction).Name} enum.");
            }
        }

        /// <summary>
        ///     Convert last action
        /// </summary>
        /// <param name="lastTriggerAction">entity</param>
        /// <returns>DTO</returns>
        /// <exception cref="FormatException">Entity enum value is not recognized.</exception>
        public static LastTriggerActionDTO ConvertLastAction(LastTriggerAction lastTriggerAction)
        {
            switch (lastTriggerAction)
            {
                case LastTriggerAction.Revoke:
                    return LastTriggerActionDTO.AbortTrigger;
                case LastTriggerAction.Grant:
                    return LastTriggerActionDTO.ExecuteActions;
                default:
                    throw new FormatException(
                        $"Value '{lastTriggerAction}' do not exist in the {typeof(LastTriggerAction).Name} enum.");
            }
        }

        /// <summary>
        ///     Convert all different types of rules to DTOs
        /// </summary>
        /// <param name="rule">entity</param>
        /// <returns>DTO</returns>
        /// <exception cref="FormatException">Subclass is not recognized.</exception>
        public static TriggerRuleBase ConvertRule(ITriggerRule rule)
        {
            switch (rule)
            {
                case ContextCollectionRule dto:
                    return new TriggerContextRule
                    {
                        ContextName = dto.ContextName,
                        Filter = ConvertFilterCondition(dto.Condition),
                        PropertyName = dto.PropertyName,
                        PropertyValue = dto.PropertyValue,
                        ResultToUse = ConvertFilterResult(dto.ResultToUse)
                    };
                case ExceptionRule dto1:
                    return new TriggerExceptionRule
                    {
                        FieldName = dto1.FieldName,
                        Filter = ConvertFilterCondition(dto1.Condition),
                        ResultToUse = ConvertFilterResult(dto1.ResultToUse),
                        Value = dto1.Value
                    };
            }

            throw new FormatException("Failed to convert " + rule);
        }
    }
}