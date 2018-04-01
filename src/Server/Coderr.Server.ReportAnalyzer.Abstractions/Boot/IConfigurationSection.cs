using System.Collections.Generic;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public interface IConfigurationSection
    {
        string this[string name] { get; }
        IEnumerable<IConfigurationSection> GetChildren();

        string Value { get; }
    }

}
