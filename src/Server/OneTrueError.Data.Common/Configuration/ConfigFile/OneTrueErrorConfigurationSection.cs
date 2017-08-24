using System.Configuration;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    ///     Main configuration section
    /// </summary>
    public class OneTrueErrorConfigurationSection : ConfigurationSection
    {
        /// <summary>
        ///     Contains all sections for different areas of OneTrueError.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SectionCollection Sections
        {
            get
            {
                var hostCollection = (SectionCollection) base[""];
                return hostCollection;
            }
        }
    }
}