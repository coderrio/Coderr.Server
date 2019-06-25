using System.Collections;
using System.Collections.Generic;

namespace Coderr.Server.Abstractions.Boot
{
    /// <summary>
    /// Abstraction for the .NET Core configuration files.
    /// </summary>
    public interface IConfigurationSection
    {
        string this[string name] { get; }
        IEnumerable<IConfigurationSection> GetChildren();

        string Value { get; }
    }

}
