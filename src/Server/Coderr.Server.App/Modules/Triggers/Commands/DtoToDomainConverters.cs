using System;
using System.Diagnostics.CodeAnalysis;
using codeRR.Server.Api.Modules.Triggers;
using codeRR.Server.App.Modules.Triggers.Domain;
using codeRR.Server.App.Modules.Triggers.Domain.Rules;

namespace codeRR.Server.App.Modules.Triggers.Commands
{
    /// <summary>
    ///     Convert DTOs to Entities.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Unknown enum values and unknown sub classes should generate exceptions (<c>FormatException</c>) so that we know
    ///         that the converts have not been updated.
    ///     </para>
    /// </remarks>
    public static class DtoToDomainConverters
    {
        /// <summary>
        ///     Convert the dto to an entity
        /// </summary>
        /// <param name="action">dto</param>
        /// <returns>entity</returns>
        public static ActionConfigurationData ConvertAction(TriggerActionDataDTO action)
        {
            if (action == null) throw new ArgumentNullException("action");
            return new ActionConfigurationData
            {
                Data = action.ActionContext,
                ActionName = action.ActionName
            };
        }

        /// <summary>
        ///     Convert the dto to an entity
        /// </summary>
        /// <param name="filter">dto</param>
        /// <returns>entity</returns>
        /// <exception cref="FormatException">Unknown enum value in entity</exception>
        public static FilterCondition ConvertFilterCondition(TriggerFilterCondition filter)
        {
            switch (filter)
            {
                case TriggerFilterCondition.Contains:
                    return FilterCondition.Contains;
                case TriggerFilterCondition.DoNotContain:
                    return FilterCondition.DoNotContain;
                case TriggerFilterCondition.EndsWith:
                    return FilterCondition.EndsWith;
                case TriggerFilterCondition.Equals:
                    return FilterCondition.Equals;
                case TriggerFilterCondition.StartsWith:
                    return FilterCondition.StartsWith;
                default:
                    throw new FormatException(string.Format((string) "Value '{0}' do not exist in the {1} enum.",
                        (object) filter, typeof(FilterCondition).Name));
            }
        }


        /// <summary>
        ///     Convert filter
        /// </summary>
        /// <param name="ruleAction">dto</param>
        /// <returns>entity</returns>
        /// <exception cref="FormatException">Entity enum contains a value that is currently not handled.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterResult")]
        public static FilterResult ConvertFilterResult(TriggerRuleAction ruleAction)
        {
            switch (ruleAction)
            {
                case TriggerRuleAction.AbortTrigger:
                    return FilterResult.Revoke;
                case TriggerRuleAction.ContinueWithNextRule:
                    return FilterResult.Continue;
                case TriggerRuleAction.ExecuteActions:
                    return FilterResult.Grant;
                default:
                    throw new FormatException(string.Format((string) "Value '{0}' do not exist in the {1} enum.",
                        (object) ruleAction, typeof(FilterResult).Name));
            }
        }

        /// <summary>
        ///     Convert last action
        /// </summary>
        /// <param name="lastTriggerAction">dto</param>
        /// <returns>entity</returns>
        /// <exception cref="FormatException">Entity enum value is not recognized.</exception>
        public static LastTriggerAction ConvertLastAction(LastTriggerActionDTO lastTriggerAction)
        {
            switch (lastTriggerAction)
            {
                case LastTriggerActionDTO.AbortTrigger:
                    return LastTriggerAction.Revoke;
                case LastTriggerActionDTO.ExecuteActions:
                    return LastTriggerAction.Grant;
                default:
                    throw new FormatException(string.Format((string) "Value '{0}' do not exist in the {1} enum.",
                        (object) lastTriggerAction, typeof(LastTriggerAction).Name));
            }
        }

        /// <summary>
        ///     Convert all different types of rules to entities
        /// </summary>
        /// <param name="rule">dto</param>
        /// <returns>entity</returns>
        /// <exception cref="FormatException">Subclass is not recognized.</exception>
        public static ITriggerRule ConvertRule(TriggerRuleBase rule)
        {
            if (rule is TriggerContextRule)
            {
                var dto = (TriggerContextRule) rule;
                return new ContextCollectionRule
                {
                    ContextName = dto.ContextName,
                    Condition = ConvertFilterCondition(dto.Filter),
                    PropertyName = dto.PropertyName,
                    PropertyValue = dto.PropertyValue,
                    ResultToUse = ConvertFilterResult(dto.ResultToUse)
                };
            }

            if (rule is TriggerExceptionRule)
            {
                var dto = (TriggerExceptionRule) rule;
                return new ExceptionRule
                {
                    FieldName = dto.FieldName,
                    Condition = ConvertFilterCondition(dto.Filter),
                    ResultToUse = ConvertFilterResult(dto.ResultToUse),
                    Value = dto.Value
                };
            }

            throw new FormatException("Failed to convert " + rule);
        }
    }
}