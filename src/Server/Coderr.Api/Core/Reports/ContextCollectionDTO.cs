using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.Api.Core.Reports
{
    /// <summary>
    ///     Context collection DTO.
    /// </summary>
    public class ContextCollectionDTO
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ContextCollectionDTO" />.
        /// </summary>
        protected ContextCollectionDTO()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ContextCollectionDTO" />.
        /// </summary>
        /// <param name="name">Name as specified in the client library</param>
        /// <param name="items">Properties.</param>
        public ContextCollectionDTO(string name, IDictionary<string, string> items)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (items == null) throw new ArgumentNullException("items");

            Name = name;
            Properties = items;
        }


        /// <summary>
        ///     Name as specified in the client library
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Properties.
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var flatten = Properties.Select(x => x.Key + "=" + x.Value);
            var joinProps = string.Join(", ", flatten);
            return string.Format("{0} [{1}]", Name, joinProps);
        }
    }
}