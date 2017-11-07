using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Contains information about a collection in a specific application
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This metadata will be used to enable autocomplete when designing triggers for the application.
    ///     </para>
    /// </remarks>
    public class CollectionMetadata
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CollectionMetadata" />.
        /// </summary>
        /// <param name="applicationId">Application identity (i.e. primary key)</param>
        /// <param name="name">Name as specified in the client library</param>
        public CollectionMetadata(int applicationId, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");

            Name = name;
            ApplicationId = applicationId;
            Properties = new List<string>();
            IsUpdated = false;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected CollectionMetadata()
        {
            IsUpdated = false;
        }

        /// <summary>
        ///     Application that the incident belongs to
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Collection identity (unique between all collections, while the name is just unique for the referenced incident).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Incident has been updated.
        /// </summary>
        [JsonIgnore]
        public bool IsUpdated { get; private set; }

        /// <summary>
        ///     Name as specified in the client library
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Properties collected by the client library.
        /// </summary>
        public ICollection<string> Properties { get; private set; }

        /// <summary>
        ///     Add or update a property.
        /// </summary>
        /// <param name="name">Property name</param>
        public void AddOrUpdateProperty(string name)
        {
            if (Properties.Contains(name))
                return;

            IsUpdated = true;
            Properties.Add(name);
        }
    }
}