using System;
using System.Collections.Generic;

namespace codeRR.Server.Infrastructure.Plugins
{
    /// <summary>
    ///     Item for <see cref="MenuConfiguration" />.
    /// </summary>
    public class MenuItem
    {
        private readonly IDictionary<string, MenuItem> _items = new Dictionary<string, MenuItem>();

        /// <summary>
        ///     Creates a new instance of <see cref="MenuItem" />.
        /// </summary>
        /// <param name="name">
        ///     menu identifier (assigned as HTML Element id), so try to use a somewhat unique name, or we'll get a
        ///     very sad face when the menu stop working.
        /// </param>
        /// <param name="title">Menu item title</param>
        /// <param name="spaRoute">Page to visit</param>
        public MenuItem(string name, string title, string spaRoute)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            SpaRoute = spaRoute ?? throw new ArgumentNullException(nameof(spaRoute));
            Name = name;
        }

        /// <summary>
        ///     Child items
        /// </summary>
        /// <param name="name">Name in pascal case</param>
        /// <returns>item</returns>
        /// <exception cref="KeyNotFoundException">name</exception>
        public MenuItem this[string name] => _items[name];

        /// <summary>
        ///     All children
        /// </summary>
        public IEnumerable<MenuItem> Items => _items.Values;

        /// <summary>
        ///     Name in PascalCase.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Hashbang path
        /// </summary>
        public string SpaRoute { get; }

        /// <summary>
        ///     Title (menu text)
        /// </summary>
        public string Title { get; }


        /// <summary>
        ///     Add child item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="link"></param>
        public void Add(string name, string title, string link)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (link == null) throw new ArgumentNullException(nameof(link));
            if (name.Contains(" "))
                throw new FormatException("Name may not contain spaces or any other stupid characters.");

            _items.Add(name, new MenuItem(name, title, link));
        }

        /// <summary>
        ///     Change path to absolute path (except for hash routes).
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public string ToAbsolute(Func<string, string> formatter)
        {
            if (!SpaRoute.StartsWith("#"))
                return formatter(SpaRoute);

            return SpaRoute;
        }
    }
}