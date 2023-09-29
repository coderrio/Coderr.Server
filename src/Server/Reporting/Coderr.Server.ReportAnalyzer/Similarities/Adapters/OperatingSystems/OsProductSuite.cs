using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Coderr.Server.ReportAnalyzer.Similarities.Adapters.OperatingSystems
{
    /// <summary>
    ///     Translates the WMI collection "OS_PRODUCT_SUITE"
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Os")]
    public static class OsProductSuite
    {
        private static readonly Dictionary<int, string> SuiteTypes = new Dictionary<int, string>
        {
            {1, "SmallBusiness"},
            {2, "Enterprise"},
            {4, "BackOffice"},
            {8, "CommunicationServer"},
            {16, "TerminalServer"},
            {32, "SmallBusinessRestricted"},
            {64, "EmbeddedNT"},
            {128, "DataCenter"},
            {256, "Terminal Services"},
            {512, "Windows Home"},
            {1024, "Web Server"},
            {8192, "Storage Server"},
            {16384, "Compute Cluster"}
        };

        /// <summary>
        ///     Get names from suite flags.
        /// </summary>
        /// <param name="suitValues">flag value</param>
        /// <returns>Matching suites delimited by new lines.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
            Justification = "Value cannot be null.")]
        public static string GetNames(string suitValues)
        {
            if (suitValues == null) throw new ArgumentNullException("suitValues");

            int chosenSuits;
            if (!int.TryParse(suitValues, out chosenSuits))
                return "";

            var result = "";
            foreach (var item1 in SuiteTypes)
            {
                if ((chosenSuits & item1.Key) != 0)
                    result += item1.Value + "\r\n";
            }

            if (result != "")
                result = result.Substring(result.Length - 2, 2);

            return result;
        }
    }
}