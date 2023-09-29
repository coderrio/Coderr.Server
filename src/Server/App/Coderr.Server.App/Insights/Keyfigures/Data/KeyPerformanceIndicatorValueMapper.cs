using System;
using Griffin.Data.Mapper;

namespace Coderr.Server.App.Insights.Keyfigures.Data
{
    public class KeyPerformanceIndicatorValueMapper : CrudEntityMapper<KeyPerformanceIndicatorValue>
    {
        public KeyPerformanceIndicatorValueMapper() : base("PremiseKPI")
        {
            Property(x => x.Value)
                .ToColumnValue(x => x.ToString())
                .ToPropertyValue2(x =>
                {
                    var type = Type.GetType((string) x.Record["ValueType"], true);
                    return Convert.ChangeType(x.Value, type);
                });

            Property(x => x.ValueType)
                .ToColumnValue2(x => x.Value.GetType().FullName);
        }
    }
}