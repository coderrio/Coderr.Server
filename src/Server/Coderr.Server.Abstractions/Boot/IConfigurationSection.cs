using System.Collections;
using System.Collections.Generic;

namespace Coderr.Server.Abstractions.Boot
{
    public interface IConfigurationSection
    {
        string this[string name] { get; }
        IEnumerable<IConfigurationSection> GetChildren();

        string Value { get; }
    }

}
