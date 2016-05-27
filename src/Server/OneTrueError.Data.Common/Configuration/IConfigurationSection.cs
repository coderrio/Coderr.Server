using System.Collections.Generic;

namespace OneTrueError.Infrastructure.Configuration
{
    /// <summary>
    ///     Purpose of this interface is to allow strongly types settings to be stored in a configuration store without
    ///     exposing magic strings.
    /// </summary>
    public interface IConfigurationSection
    {
        string SectionName { get; }

        IDictionary<string, string> ToDictionary();

        void Load(IDictionary<string, string> settings);
    }
}