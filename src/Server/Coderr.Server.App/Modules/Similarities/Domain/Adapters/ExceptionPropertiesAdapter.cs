using System;
using System.Linq;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    internal class ExceptionPropertiesAdapter : IValueAdapter
    {
        private readonly string[] IgnoredProperties =
        {
            "Message", "StackTrace", "InnerException", "TargetSite.",
            "TargetSite"
        };

        public object Adapt(ValueAdapterContext context, object currentValue)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!context.ContextName.Equals("ExceptionProperties", StringComparison.OrdinalIgnoreCase))
                return currentValue;

            if (IgnoredProperties.Any(x => x.Equals(context.PropertyName, StringComparison.OrdinalIgnoreCase)))
            {
                context.IgnoreProperty = true;
                return currentValue;
            }

            return currentValue;
        }
    }
}