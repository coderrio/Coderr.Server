using System;
using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.ReportAnalyzer.Similarities.Adapters.Old
{
    internal class ProcessorTimeAdapter : IValueAdapter
    {
        private static readonly string[] Fields = {"TotalProcessorTime", "UserProcessorTime"};

        public object Adapt(ValueAdapterContext context, object currentValue)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (context.ContextName != "ApplicationInfo")
                return currentValue;

            if (!Fields.Contains(context.PropertyName, EqualityComparer<string>.Default))
                return currentValue;

            var timeSpan = TimeSpan.Parse(context.Value.ToString());

            if (timeSpan < TimeSpan.FromMinutes(1))
                return "under one minute";
            if (timeSpan < TimeSpan.FromMinutes(30))
                return "between 1 and 30 minutes";
            if (timeSpan < TimeSpan.FromHours(1))
                return "between 30 minutes and an hour";
            if (timeSpan < TimeSpan.FromDays(1))
                return "between an hour and a day";
            if (timeSpan < TimeSpan.FromDays(30))
                return "up to a month";
            if (timeSpan < TimeSpan.FromDays(90))
                return "between one and three months";
            if (timeSpan > TimeSpan.FromDays(365/2))
                return "between three and six months";
            if (timeSpan > TimeSpan.FromDays(365/2))
                return "between 6 months and a year";

            return "up to " + (int) timeSpan.TotalDays/365 + " years";
        }
    }
}