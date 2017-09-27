using System;
using System.Linq;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Normalizers;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Generates the "ApplicationInfo" context collection from information in different uploaded collections.
    /// </summary>
    public class ApplicationInfoAdapter : IValueAdapter
    {
        private static readonly string[] MemoryProperties = {"WorkingSet", "VirtualMemorySize", "PrivateMemorySize"};

        /// <summary>
        ///     Adapt the value specified in the context
        /// </summary>
        /// <param name="context">Context information</param>
        /// <param name="currentValue">Value which might have been adapted</param>
        /// <returns>The new value (or same as the current value if no modification has been made)</returns>
        public object Adapt(ValueAdapterContext context, object currentValue)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (currentValue == null || context.ContextName != "ApplicationInfo")
                return currentValue;

            if (context.PropertyName == "MainModule")
            {
                if (currentValue.ToString().StartsWith("System.Diagnostics.ProcessModule"))
                    return currentValue.ToString()
                        .Substring("System.Diagnostics.ProcessModule".Length)
                        .Trim(' ', '(', ')');
            }

            if (MemoryProperties.Any(x => x.Equals(context.PropertyName, StringComparison.OrdinalIgnoreCase)))
            {
                var value = 0;
                if (!int.TryParse(currentValue.ToString(), out value))
                {
                    return currentValue;
                }

                value = value/1000000;
                return MemoryNormalizer.Divide(value, context.TypeOfApplication == "Mobile" ? 32 : 512);
            }

            if (context.PropertyName == "ThreadCount")
            {
                return NumberNormalizer.Normalize(currentValue.ToString(), 10, 50);
            }
            if (context.PropertyName == "HandleCount")
            {
                return NumberNormalizer.Normalize(currentValue.ToString(), 100, 5000);
            }

            if (context.PropertyName == "StartTime")
            {
                DateTime dt;
                if (!DateTime.TryParse(currentValue.ToString(), out dt))
                    return currentValue;

                context.IgnoreProperty = true;
                context.AddCustomField("ApplicationInfo", "StartTime.Hour", dt.Hour);
                context.AddCustomField("ApplicationInfo", "StartTime.DayOfWeek", dt.DayOfWeek.ToString());
            }

            context.IgnoreProperty = true;
            return currentValue;
        }
    }
}