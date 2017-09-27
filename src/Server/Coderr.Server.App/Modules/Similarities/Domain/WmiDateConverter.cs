using System;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using log4net;

namespace codeRR.Server.App.Modules.Similarities.Domain
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
                result = ManagementDateTimeConverter.ToDateTime(date);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to convert " + date + ".", ex);
            }

            return DateTime.TryParse(date, out result);
        }
    }
}