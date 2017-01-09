using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    ///     A configuration element used to represent a keu/value collections
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class KeyValueCollection : ConfigurationElementCollection
    {
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
        ///     "setting"
        /// </summary>
        protected override string ElementName
        {
            get { return "setting"; }
        }

        /// <summary>
        ///     Get a key value pair.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>element if found; otherwise <c>null</c>.</returns>
        public new KeyValueElement this[string key]
        {
            get
            {
                if (IndexOf(key) < 0) return null;
                return (KeyValueElement) BaseGet(key);
            }
        }

        /// <summary>
        ///     Get element from index
        /// </summary>
        /// <param name="index">zero based index</param>
        /// <returns>Element</returns>
        public KeyValueElement this[int index]
        {
            get { return (KeyValueElement) BaseGet(index); }
        }

        /// <summary>
        ///     Add a new element
        /// </summary>
        /// <param name="item">item</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void Add(KeyValueElement item)
        {
            if (item == null) throw new ArgumentNullException("item");
            BaseAdd(item);
        }

        /// <summary>
        ///     When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        ///     A newly created <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyValueElement();
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
            return ((KeyValueElement) element).Key;
        }


        private int IndexOf(string name)
        {
            name = name.ToLower();

            for (var idx = 0; idx < Count; idx++)
            {
                if (this[idx].Key.ToLower() == name)
                    return idx;
            }
            return -1;
        }
    }
}