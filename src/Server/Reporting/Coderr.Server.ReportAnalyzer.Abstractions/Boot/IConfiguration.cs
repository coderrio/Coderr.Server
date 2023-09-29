using System.Collections.Generic;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public interface IConfiguration
    {
        string this[string name] { get; }
        IEnumerable<IConfigurationSection> GetChildren();
        string GetConnectionString(string name);
        IConfigurationSection GetSection(string name);
    }
}