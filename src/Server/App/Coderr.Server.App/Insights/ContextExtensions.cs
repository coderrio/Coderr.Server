using System;
using Coderr.Server.App.Insights.Keyfigures;

namespace Coderr.Server.App.Insights
{
    public static class ContextExtensions
    {
        public static bool IsInPeriod2(this KeyPerformanceIndicatorContext context, DateTime start, DateTime? end)
        {
            return start <= context.PeriodEndDate && (end == null || end >= context.PeriodStartDate);
        }
        public static bool IsInValueInterval2(this KeyPerformanceIndicatorContext context, DateTime start, DateTime? end)
        {
            return start <= context.ValueEndDate && (end == null || end >= context.ValueStartDate);
        }

        public static int PeriodDurationInMonths(this KeyPerformanceIndicatorContext context)
        {
            return (int)(context.PeriodEndDate.Subtract(context.PeriodStartDate).TotalDays / 30);
        }
    }
}
