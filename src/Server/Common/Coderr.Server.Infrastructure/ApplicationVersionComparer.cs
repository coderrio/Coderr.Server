using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.Infrastructure
{
    public class ApplicationVersionComparer : IComparer<string>
    {
        public int Compare(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
                return 0;

            int value;

            var pos = first.IndexOf('-');
            if (pos != -1)
                first = first.Substring(0, pos);
            var firstVersion = first.Split('.').Select(y => int.TryParse(y, out value) ? value : -1).ToArray();
            if (firstVersion.Any(x => x == -1))
                return 0;

            pos = second.IndexOf('-');
            if (pos != -1)
                second = second.Substring(0, pos);
            var secondVersion = second.Split('.').Select(y => int.TryParse(y, out value) ? value : -1).ToArray();
            if (secondVersion.Any(x => x == -1))
                return 0;

            for (var i = 0; i < firstVersion.Length; i++)
            {
                if (secondVersion.Length <= i)
                    return 1;

                if (firstVersion[i] < secondVersion[i])
                    return -1;
                if (firstVersion[i] > secondVersion[i])
                    return 1;
            }

            if (firstVersion.Length < secondVersion.Length)
                return -1;

            return 0;
        }
    }
}