using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Similarities.Handlers.Processing
{
    /// <summary>
    ///     Translates WMI dates to .NETs DateTime.
    /// </summary>
    public static class WmiDateConverter
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WmiDateConverter));

        /// <summary>
        ///     Try parse a WMI date
        /// </summary>
        /// <param name="date">date</param>
        /// <param name="result">converted date</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">date</exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public static bool TryParse(string date, out DateTime result)
    {
        if (date == null) throw new ArgumentNullException("date");

        if (date.Length < 22 || date.Length > 26 || date[14] != '.' || date[0] != '2' || date[1] != '0')
            return DateTime.TryParse(date, out result);

        try
        {
            var timezonePos = date.IndexOfAny(new[]{'+', '-'});
            var isPlus = date[timezonePos] == '+';
            var timeZone = date.Substring(timezonePos + 1);
            date = date.Substring(0, timezonePos);

            var date2 = DateTime.ParseExact(date, "yyyyMMddHHmmss.ffffff", CultureInfo.InvariantCulture);
            result = date2;

            var timeZoneMinutes = int.Parse(timeZone);
            //get utc by remving timezone adjustment
            result = isPlus
                ? date2.AddMinutes(-timeZoneMinutes)
                : date2.AddMinutes(timeZoneMinutes);
                
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to convert {date}.", ex);
        }

        return DateTime.TryParse(date, out result);
    }
    }
}