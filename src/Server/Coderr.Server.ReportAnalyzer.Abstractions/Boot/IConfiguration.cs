using System.Collections.Generic;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public interface IConfiguration
    {
        IEnumerable<IConfigurationSection> GetChildren();
        string GetConnectionString(string name);
        IConfigurationSection GetSection(string name);
    }
}