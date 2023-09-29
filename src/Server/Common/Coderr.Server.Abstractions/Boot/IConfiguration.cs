using System.Collections.Generic;

namespace Coderr.Server.Abstractions.Boot
{
    public interface IConfiguration 
    {
        string this[string name] { get; }
        IConfigurationSection GetSection(string name);
        string GetConnectionString(string name);
        IEnumerable<IConfigurationSection> GetChildren();
    }

}
