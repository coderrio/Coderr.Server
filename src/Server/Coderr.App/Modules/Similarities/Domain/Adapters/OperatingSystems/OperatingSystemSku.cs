using System.Collections.Generic;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters.OperatingSystems
{
    /// <summary>
    ///     Translates the WMI collection named "OperatingSystemSKU"
    /// </summary>
    public static class OperatingSystemSku
    {
        private static readonly Dictionary<int, string> Editions = new Dictionary<int, string>
        {
            {1, "Ultimate"},
            {2, "Home Basic"},
            {3, "Home Premium"},
            {4, "Ent"},
            {5, "Home Basic N"},
            {6, "Business"},
            {7, "Server Std"},
            {8, "Server DC (full)"},
            {9, "Windows SBS"},
            {10, "Server Ent (full)"},
            {11, "Starter"},
            {12, "Server DC (core)"},
            {13, "Server Std (core)"},
            {14, "Server Ent (core)"},
            {15, "Server Ent for Itanium-based Systems"},
            {16, "Business N"},
            {17, "Web Server (full)"},
            {18, "HPC Edition"},
            {19, "Windows Storage Server 2008 R2 Essentials"},
            {20, "Storage Server Express"},
            {21, "Storage Server Std"},
            {22, "Storage Server Workgroup"},
            {23, "Storage Server Ent"},
            {24, "Windows Server 2008 for Windows Essential Server Solutions"},
            {25, "SBS Premium"},
            {26, "Home Premium N"},
            {27, "Enterprise N"},
            {28, "Ultimate N"},
            {29, "Web Server (core)"},
            {30, "Windows Essential Business Server Management Server"},
            {31, "Windows Essential Business Server Security Server"},
            {32, "Windows Essential Business Server Messaging Server"},
            {33, "Server Foundation"},
            {34, "Windows Home Server 2011"},
            {35, "Windows Server 2008 w/o Hyper-V for Windows Essential Server Solutions"},
            {36, "Server Std w/o Hyper-V"},
            {37, "Server DC w/o Hyper-V (full)"},
            {38, "Server Ent w/o Hyper-V (full)"},
            {39, "Server DC w/o Hyper-V (core)"},
            {40, "Server Std w/o Hyper-V (core)"},
            {41, "Server Ent w/o Hyper-V (core)"},
            {42, "Microsoft Hyper-V Server"},
            {43, "Storage Server Express (core)"},
            {44, "Storage Server Std (core)"},
            {45, "Storage Server Workgroup (core)"},
            {46, "Storage Server Ent (core)"},
            {47, "Starter N"},
            {48, "Professional"},
            {49, "Professional N"},
            {50, "Windows SBS 2011 Essentials"},
            {51, "Server For SB Solutions"},
            {52, "Server Solutions Premium"},
            {53, "Server Solutions Premium (core)"},
            {54, "Server For SB Solutions EM"},
            {55, "Server For SB Solutions EM"},
            {56, "Windows MultiPoint Server"},
            {59, "Windows Essential Server Solution Management"},
            {60, "Windows Essential Server Solution Additional"},
            {61, "Windows Essential Server Solution Management SVC"},
            {62, "Windows Essential Server Solution Additional SVC"},
            {63, "SBS Premium (core)"},
            {64, "Server Hyper Core V"},
            {66, "Starter E"},
            {67, "Home Basic E"},
            {68, "Home Premium E"},
            {69, "Professional E"},
            {70, "Enterprise E"},
            {71, "Ultimate E"},
            {72, "Server Ent (evaluation)"},
            {76, "Windows MultiPoint Server Std (full)"},
            {77, "Windows MultiPoint Server Premium (full)"},
            {79, "Server Std (evaluation)"},
            {80, "Server DC (evaluation)"},
            {84, "Enterprise N (evaluation)"},
            {95, "Storage Server Workgroup (evaluation)"},
            {96, "Storage Server Std (evaluation)"},
            {98, "Windows 8 N"},
            {99, "Windows 8 China"},
            {100, "Windows 8 Single Language"},
            {101, "Windows 8"},
            {103, "Professional with Media Center"}
        };

        /// <summary>
        ///     Get edition from a suite index
        /// </summary>
        /// <param name="id">index</param>
        /// <returns>corresponding suite if found; otherwise <c>"Unknown"</c></returns>
        public static string GetName(string id)
        {
            int idInt;
            if (!int.TryParse(id, out idInt))
                return "Unknown";

            string item;
            return Editions.TryGetValue(idInt, out item) ? item : "Unknown";
        }
    }
}