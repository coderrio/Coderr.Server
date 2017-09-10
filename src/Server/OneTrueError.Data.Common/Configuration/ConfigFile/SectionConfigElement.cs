using System.Configuration;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    ///     Configuration element representation a OTE config section
    /// </summary>
    public class SectionConfigElement : ConfigurationElement
    {
        /// <summary>
        ///     Name of this section
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = "  ~!@#$%^&*()[]{}/;’\"|\\")]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        ///     Key value collection accessor.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public KeyValueCollection Settings
        {
            get { return (KeyValueCollection) base[""]; }
        }
    }
}