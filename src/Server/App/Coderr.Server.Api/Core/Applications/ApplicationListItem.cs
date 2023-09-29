using System;
using Coderr.Server.Api.Core.Applications.Queries;

namespace Coderr.Server.Api.Core.Applications
{
    /// <summary>
    ///     Result item for <see cref="GetApplicationList" />
    /// </summary>
    public class ApplicationListItem
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationListItem" />.
        /// </summary>
        /// <param name="id">application identity</param>
        /// <param name="name">name of the application</param>
        public ApplicationListItem(int id, string name)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");

            Id = id;
            Name = name ?? throw new ArgumentNullException("name");
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ApplicationListItem()
        {
        }

        /// <summary>
        ///     Group that this application belongs to.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        ///     Name of the group
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///     Id of the application (primary key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     User that requested this list is the admin of the specified application.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Application name as entered by the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Number of full time developers.
        /// </summary>
        /// <value>
        ///     null = not specified
        /// </value>
        public decimal? NumberOfDevelopers { get; set; }
    }
}