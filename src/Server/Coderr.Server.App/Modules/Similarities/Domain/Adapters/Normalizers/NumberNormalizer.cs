using System;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters.Normalizers
{
    internal class NumberNormalizer
    {
        public static string Normalize(string value, float step, int max)
        {
            if (value == null)
                return null;

            var value2 = 0;
            if (!int.TryParse(value, out value2))
                return null;

            if (value2 < 10)
                return "Less than 10";

            if (value2 >= max)
                return "> " + max;

            var lowerValue = (int) Math.Floor(value2/step)*(int) step;
            return "Between " + lowerValue + " and " + (lowerValue + (int) step);
        }
    }
}