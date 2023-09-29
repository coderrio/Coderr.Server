using System;

namespace Coderr.Server.App.Insights
{
    public static class DecimalExtensions
    {
        public static int Rounded(this decimal? value)
        {
            if (value == null)
                return 0;

            return (int)Math.Round(value.Value);
        }
        public static int Rounded(this double? value)
        {
            if (value == null)
                return 0;

            return (int)Math.Round(value.Value);
        }
        public static int Rounded(this double value)
        {
            return (int)Math.Round(value);
        }
        public static int Ceiling(this double value)
        {
            return (int)Math.Ceiling(value);
        }
    }
}
