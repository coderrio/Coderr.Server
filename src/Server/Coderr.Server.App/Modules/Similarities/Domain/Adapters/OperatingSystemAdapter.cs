using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Normalizers;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.OperatingSystems;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Converts the Operating system WMI collection into more useful information.
    /// </summary>
    /// TODO: Document which collections this one generates.
    //http://www.powertheshell.com/reference/wmireference/root/cimv2/win32_operatingsystem/
    public class OperatingSystemAdapter : IValueAdapter
    {
        private static readonly string[] LocalizationProperties =
        {
            "CurrentTimeZone",
            "CountryCode",
            "MUILanguages", "CodeSet", "Locale", "OSLanguage"
        };

        private static readonly string[] OsEnvironment =
        {
            "NumberOfUsers", "SizeStoredInPagingFiles", "SystemDevice", "SystemDirectory", "SystemDrive",
            "TotalVirtualMemorySize", "TotalVisibleMemorySize", "WindowsDirectory", "NumberOfProcesses", "NumberOfUsers",
            "FreePhysicalMemory", "FreeSpaceInPagingFiles", "FreeVirtualMemory"
        };

        //private string[] _allowedProperties =
        //{
        //    "Primary", "PortableOperatingSystem", "Version",
        //    "Caption", "OSArchitecture", "BuildNumber", "BuildType"
        //};

        //TODO: Refactor code below into multiple methods and use the dictionary
        //to invoke the correct property handler.

        /// <summary>
        ///     Adapt the value specified in the context
        /// </summary>
        /// <param name="context">Context information</param>
        /// <param name="currentValue">Value which might have been adapted</param>
        /// <returns>The new value (or same as the current value if no modification has been made)</returns>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
            Justification = "Value cannot be null.")]
        public object Adapt(ValueAdapterContext context, object currentValue)
        {
            if (context == null) throw new ArgumentNullException("context");
            var val = currentValue as string;
            if (string.IsNullOrEmpty(val))
                return currentValue;
            if (!context.ContextName.Equals("OperatingSystem", StringComparison.OrdinalIgnoreCase))
                return currentValue;

            context.IgnoreProperty = true;
            if (context.PropertyName.Equals("OperatingSystemSKU", StringComparison.OrdinalIgnoreCase))
            {
                var name = OperatingSystemSku.GetName(currentValue.ToString()) ?? currentValue;
                context.AddCustomField("OS.Metadata", "Edition", name);
            }

            if (context.PropertyName.Equals("CSDVersion", StringComparison.OrdinalIgnoreCase))
            {
                context.AddCustomField("OS.Metadata", "ServicePack", currentValue.ToString());
                return currentValue;
            }

            if (context.PropertyName.Equals("OSProductSuite", StringComparison.OrdinalIgnoreCase))
            {
                var value = OsProductSuite.GetNames(currentValue.ToString()) ?? currentValue;
                context.AddCustomField("OS.Metadata", "ProductSuite", value);
                return currentValue;
            }

            if (context.PropertyName.Equals("ProductType", StringComparison.OrdinalIgnoreCase))
            {
                switch (currentValue.ToString())
                {
                    case "1":
                        currentValue = "Work Station";
                        break;
                    case "2":
                        currentValue = "Domain Controller";
                        break;
                    case "3":
                        currentValue = "Server";
                        break;
                }
                context.AddCustomField("OS.Metadata", "ProductType", currentValue);
                return currentValue;
            }


            if (context.PropertyName.Equals("SuiteMask", StringComparison.OrdinalIgnoreCase))
            {
                var bitMask = 0;
                var value = "";
                if (int.TryParse(currentValue.ToString(), out bitMask))
                {
                    if ((bitMask & 1) != 0)
                        value += "Small Business, ";
                    if ((bitMask & 2) != 0)
                        value += "Enterprise, ";
                    if ((bitMask & 4) != 0)
                        value += "BackOffice, ";
                    if ((bitMask & 8) != 0)
                        value += "Communications, ";
                    if ((bitMask & 16) != 0)
                        value += "Terminal Services, ";
                    if ((bitMask & 32) != 0)
                        value += "Small Business Restricted, ";
                    if ((bitMask & 64) != 0)
                        value += "Embedded Edition, ";
                    if ((bitMask & 128) != 0)
                        value += "Datacenter Edition, ";
                    if ((bitMask & 256) != 0)
                        value += "Single User, ";
                    if ((bitMask & 512) != 0)
                        value += "Home Edition, ";
                    if ((bitMask & 1024) != 0)
                        value += "Web Server Edition, ";
                }
                if (value != "")
                    currentValue = value.Remove(value.Length - 2, 2);
                context.AddCustomField("OS.Metadata", "Suite2", currentValue);
                return currentValue;
            }
            if (context.PropertyName.Equals("QuantumLength", StringComparison.OrdinalIgnoreCase))
            {
                switch (currentValue.ToString())
                {
                    case "0":
                        currentValue = "Unknown ";
                        break;
                    case "1":
                        currentValue = "One tick";
                        break;
                    case "2":
                        currentValue = "Two ticks";
                        break;
                }
                context.AddCustomField("OS.Metadata", "QuantumLength", currentValue);
                return currentValue;
            }

            if (context.PropertyName.Equals("QuantumType", StringComparison.OrdinalIgnoreCase))
            {
                switch (currentValue.ToString())
                {
                    case "0":
                        currentValue = "Unknown ";
                        break;
                    case "1":
                        currentValue = "Fixed";
                        break;
                    case "2":
                        currentValue = "Variable";
                        break;
                }
                context.AddCustomField("OS.Metadata", "QuantumType", currentValue);
                return currentValue;
            }


            if (context.PropertyName == "InstallDate")
            {
                return AdaptInstallDate(context, currentValue);
            }


            if (context.PropertyName == "LastBootUpTime")
            {
                DateTime time;
                if (WmiDateConverter.TryParse(context.Value.ToString(), out time))
                {
                    context.AddCustomField("OS.Environment", "LastBootup.Hour", time.Hour);
                    context.AddCustomField("OS.Environment", "LastBootup.DayOfWeek", time.DayOfWeek.ToString());
                }
            }


            if (context.PropertyName == "LocalDateTime")
            {
                return AdaptLocalTime(context, currentValue);
            }

            if (context.PropertyName.StartsWith("Free", StringComparison.OrdinalIgnoreCase) ||
                context.PropertyName.StartsWith("Total", StringComparison.OrdinalIgnoreCase) ||
                context.PropertyName.Equals("SizeStoredInPagingFiles", StringComparison.OrdinalIgnoreCase))
            {
                var divisor = context.TypeOfApplication == "Server" ? 512 : 256;
                var value = MemoryNormalizer.Divide(currentValue as string, divisor);
                if (!string.IsNullOrEmpty(value))
                    context.AddCustomField("OS.Environment", context.PropertyName, value);

                return currentValue;
            }

            //allow as-is
            if (context.PropertyName.StartsWith("DataExecutionPrevention", StringComparison.OrdinalIgnoreCase))
            {
                context.AddCustomField("OS.Metadata", context.PropertyName, currentValue);
                return currentValue;
            }

            if (LocalizationProperties.Any(x => x.Equals(context.PropertyName, StringComparison.OrdinalIgnoreCase)))
            {
                if (context.PropertyName == "Locale")
                {
                    int lcid;
                    if (int.TryParse(currentValue.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo,
                        out lcid))
                    {
                        currentValue = CultureInfo.GetCultureInfo(lcid).Name;
                    }
                }

                context.AddCustomField("OS.Localization", context.PropertyName, currentValue);
                return null;
            }

            if (OsEnvironment.Any(x => x.Equals(context.PropertyName, StringComparison.OrdinalIgnoreCase)))
            {
                if (context.PropertyName == "NumberOfUsers")
                {
                    if ("0".Equals(currentValue))
                        currentValue = "0";
                    else if ("1".Equals(currentValue))
                        currentValue = "1";
                    else
                        currentValue = "> 1";
                    context.AddCustomField("OS.Environment", "NumberOfLoggedInUsers", currentValue);
                    return currentValue;
                }

                if (context.PropertyName == "NumberOfProcesses")
                {
                    currentValue = NumberNormalizer.Normalize(currentValue as string, 20, 1000);
                }

                context.AddCustomField("OS.Environment", context.PropertyName, currentValue);
                return currentValue;
            }

            return currentValue;
        }

        private static object AdaptInstallDate(ValueAdapterContext context, object currentValue)
        {
            DateTime time;
            if (WmiDateConverter.TryParse(context.Value.ToString(), out time))
            {
                context.AddCustomField("OS.Environment", "InstallDate.Year", time.Year.ToString());
                context.AddCustomField("OS.Environment", "InstallDate.YearMonth", time.ToString("yyyy-MM"));
            }

            context.IgnoreProperty = true;
            return currentValue;
        }

        private static object AdaptLocalTime(ValueAdapterContext context, object currentValue)
        {
            var val = context.Value.ToString();
            var pos = val.IndexOfAny(new[] {'-', '+'});
            if (pos != -1)
            {
                var pos2 = val.IndexOfAny(new[] {'-', ' '}, pos + 1);
                if (pos2 == -1)
                    context.AddCustomField("OS.Localization", "LocalDateTime.TimeZone", val.Substring(pos));
            }


            DateTime time;
            if (WmiDateConverter.TryParse(context.Value.ToString(), out time))
            {
                context.AddCustomField("OS.Localization", "LocalDateTime.Hour", time.Hour.ToString());
                context.AddCustomField("OS.Localization", "LocalDateTime.Minute", time.Minute.ToString());
                context.AddCustomField("OS.Localization", "LocalDateTime.DayOfWeek", time.DayOfWeek.ToString());
                context.AddCustomField("OS.Localization", "LocalDateTime.DayOfYear", time.DayOfYear);
            }

            context.IgnoreProperty = true;
            return currentValue;
        }
    }
}