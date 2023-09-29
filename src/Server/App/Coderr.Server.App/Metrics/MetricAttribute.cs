using System;

namespace Coderr.Server.App.Metrics
{
    public class MetricAttribute : Attribute
    {
        public string Name { get; }

        public MetricAttribute(string name)
        {
            Name = name;
        }
    }
}