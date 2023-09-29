using System;

namespace Coderr.Server.App.Insights.Metrics
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyMetricDefinitionAttribute : Attribute
    {
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }

        public KeyMetricDefinitionAttribute(int id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }

    }
}