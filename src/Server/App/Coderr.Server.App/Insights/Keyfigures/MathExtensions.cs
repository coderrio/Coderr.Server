using System;
using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.App.Insights.Keyfigures
{
    public static class MathExtensions
    {
        public static int DivideWith(this int first, int second)
        {
            if (first == 0 || second == 0)
                return 0;

            return first / second;
        }

        public static int DivideWith(this int first, double second)
        {
            if (first == 0 || second < 1)
                return 0;

            return (int)(first / second);
        }

        public static int DivideWith(this int? first, double second)
        {
            if (first == 0 || second < 1 || first == null)
                return 0;

            return (int)(first / second);
        }

        public static int DivideWith(this double? first, double second)
        {
            if (first == null || first < 1 || second < 1)
                return 0;

            return (int)(first / second);
        }
        public static int DivideWith(this double first, double second)
        {
            if (first < 1 || second < 1)
                return 0;

            return (int)(first / second);
        }

        public static int Average<TItem>(this IEnumerable<TItem> collection, Func<TItem, int> selector,
            int defaultValue)
        {
            if (!collection.Any())
                return defaultValue;

            return (int)collection.Average(selector);
        }

        public static double Average<TItem>(this IEnumerable<TItem> collection, Func<TItem, double> selector,
            double defaultValue)
        {
            if (!collection.Any())
                return defaultValue;

            return collection.Average(selector);
        }
    }
}