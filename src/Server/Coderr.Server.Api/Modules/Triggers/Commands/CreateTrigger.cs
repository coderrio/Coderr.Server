using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Triggers.Commands
{
    /// <summary>
    ///     Create a new trigger
    /// </summary>
    [Message]
    public class CreateTrigger
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CreateTrigger" />.
        /// </summary>
        /// <param name="applicationId">Application that the trigger is for.</param>
        /// <param name="name">Trigger name</param>
        public CreateTrigger(int applicationId, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");

            ApplicationId = applicationId;
            Name = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected CreateTrigger()
        {
        }

        /// <summary>
        ///     Actions to run
        /// </summary>
        public TriggerActionDataDTO[] Actions { get; set; }

        /// <summary>
        ///     Application that the trigger belongs to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     What the trigger does and why
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Action to take after all <see cref="Actions" /> have run.
        /// </summary>
        public LastTriggerActionDTO LastTriggerAction { get; set; }


        /// <summary>
        ///     Trigger name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Rules that determine if this trigger can run.
        /// </summary>
        public TriggerRuleBase[] Rules { get; set; }

        /// <summary>
        ///     Run trigger for existing incidents (received a duplicate exception)
        /// </summary>
        public bool RunForExistingIncidents { get; set; }

        /// <summary>
        ///     Run trigger for new incidents (i.e. received a new unique exception)
        /// </summary>
        public bool RunForNewIncidents { get; set; }

        /// <summary>
        ///     Run for incidents that is closed but received a new error report.
        /// </summary>
        public bool RunForReOpenedIncidents { get; set; }
    }
}