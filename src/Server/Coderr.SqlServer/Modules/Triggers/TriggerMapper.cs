using System;
using System.Collections.Generic;
using Griffin.Data.Mapper;
using codeRR.App.Modules.Triggers.Domain;
using codeRR.SqlServer.Tools;

namespace codeRR.SqlServer.Modules.Triggers
{
    public class TriggerMapper : CrudEntityMapper<Trigger>
    {
        public TriggerMapper() : base("Triggers")
        {
            Property(x => x.LastTriggerAction)
                .ToPropertyValue(o => (LastTriggerAction) Enum.Parse(typeof(LastTriggerAction), o.ToString()))
                .ToColumnValue(o => o.ToString());


            Property(x => x.Actions)
                .ToPropertyValue(o => EntitySerializer.Deserialize<IEnumerable<ActionConfigurationData>>((string) o))
                .ToColumnValue(o => o.ToString());

            Property(x => x.Rules)
                .ToPropertyValue(o => EntitySerializer.Deserialize<IEnumerable<ITriggerRule>>((string) o))
                .ToColumnValue(o => o.ToString());

            Property(x => x.RunForExistingIncidents)
                .ToPropertyValue(Convert.ToBoolean);

            Property(x => x.RunForNewIncidents)
                .ToPropertyValue(Convert.ToBoolean);

            Property(x => x.RunForReopenedIncidents)
                .ToPropertyValue(Convert.ToBoolean);
        }
    }
}