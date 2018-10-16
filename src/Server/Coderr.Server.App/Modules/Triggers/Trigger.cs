using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Coderr.Server.App.Modules.Triggers.Rules;

namespace Coderr.Server.App.Modules.Triggers
{
    /// <summary>
    ///     A filter which decides if a notification could be sent.
    /// </summary>
    public class Trigger
    {
        private List<ActionConfigurationData> _actions = new List<ActionConfigurationData>();
        private List<RuleBase> _rules = new List<RuleBase>();

        /// <summary>
        ///     Creates a new instance of <see cref="Trigger" />.
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public Trigger(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Trigger()
        {
        }

        /// <summary>
        ///     Actions to take when the rules have been passed.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Loaded by repos")]
        public IEnumerable<ActionConfigurationData> Actions
        {
            get { return _actions; }
            private set { _actions = new List<ActionConfigurationData>(value); }
        }

        /// <summary>
        ///     Application id
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Why the trigger was created and what it does
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        ///     Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     If no filters match, do this.
        /// </summary>
        public LastTriggerAction LastTriggerAction { get; set; }

        /// <summary>
        ///     Trigger name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Rules to check
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Loaded by repos")]
        public IEnumerable<RuleBase> Rules
        {
            get { return _rules; }
            private set { _rules = new List<RuleBase>(value); }
        }

        /// <summary>
        ///     Run when we get a report for an existing incident.
        /// </summary>
        public bool RunForExistingIncidents { get; set; }

        /// <summary>
        ///     Should run for new incidents (receives a new unique exception)
        /// </summary>
        public bool RunForNewIncidents { get; set; }

        /// <summary>
        ///     Run for closed incidents that receive a new report.
        /// </summary>
        public bool RunForReopenedIncidents { get; set; }

        /// <summary>
        ///     Add a new action
        /// </summary>
        /// <param name="actionData">what to do</param>
        public void AddAction(ActionConfigurationData actionData)
        {
            if (actionData == null) throw new ArgumentNullException("actionData");
            _actions.Add(actionData);
        }


        /// <summary>
        ///     Add the rules in the order that they should be check in. the first rule added is the first rule that will decide
        ///     which action to take.
        /// </summary>
        /// <param name="rule">Rule to add</param>
        public void AddRule(RuleBase rule)
        {
            if (rule == null) throw new ArgumentNullException("rule");

            _rules.Add(rule);
        }


        /// <summary>
        ///     Remove all actions
        /// </summary>
        public void RemoveActions()
        {
            _actions.Clear();
        }

        /// <summary>
        ///     Remove all rules.
        /// </summary>
        public void RemoveRules()
        {
            _rules.Clear();
        }
    }
}
