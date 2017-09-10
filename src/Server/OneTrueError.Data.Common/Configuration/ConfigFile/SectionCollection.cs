using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    ///     Contains key value pairs
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class SectionCollection : ConfigurationElementCollection
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SectionCollection" />.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
            Justification = "Value cannot be null.")]
        public SectionCollection()
        {
            var details = (SectionConfigElement) CreateNewElement();
            if (details.Name != "")
            {
                Add(details);
            }
        }

        /// <summary>
        ///     Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection.
        /// </returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        ///     "section"
        /// </summary>
        protected override string ElementName
        {
            get { return "section"; }
        }

        /// <summary>
        ///     Gives access to a specific section
        /// </summary>
        /// <param name="index">zero based index</param>
        public SectionConfigElement this[int index]
        {
            get { return (SectionConfigElement) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        ///     Gives access to a section
        /// </summary>
        /// <param name="name">section name</param>
        /// <returns>section</returns>
        public new SectionConfigElement this[string name]
        {
            get { return (SectionConfigElement) BaseGet(name); }
        }

        /// <summary>
        ///     Add a section
        /// </summary>
        /// <param name="details">section</param>
        public void Add(SectionConfigElement details)
        {
            BaseAdd(details);
        }

        /// <summary>
        ///     Remove all sections
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        ///     Get index of the specified section
        /// </summary>
        /// <param name="details">section</param>
        /// <returns>index, -1 = not found</returns>
        public int IndexOf(SectionConfigElement details)
        {
            return BaseIndexOf(details);
        }

        /// <summary>
        ///     Remove section (if found).
        /// </summary>
        /// <param name="details">section</param>
        public void Remove(SectionConfigElement details)
        {
            if (details == null) throw new ArgumentNullException("details");
            if (BaseIndexOf(details) >= 0)
                BaseRemove(details.Name);
        }

        /// <summary>
        ///     Remove section (if found).
        /// </summary>
        /// <param name="name">section name</param>
        public void Remove(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            BaseRemove(name);
        }

        /// <summary>
        ///     Remove section at the given position
        /// </summary>
        /// <param name="index">zero based index</param>
        public void RemoveAt(int index)
        {
            if (index <= 0) throw new ArgumentOutOfRangeException("index");
            BaseRemoveAt(index);
        }


        /// <summary>
        ///     Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            BaseAdd(element, false);
        }

        /// <summary>
        ///     When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        ///     A newly created <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SectionConfigElement();
        }

        /// <summary>
        ///     Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Object" /> that acts as the key for the specified
        ///     <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SectionConfigElement) element).Name;
        }
    }
}