using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Delete an existing application including of all its data.
    /// </summary>
    [Message]
    public class DeleteApplication
    {
        /// <summary>
        ///     Creates a new instance of <see cref="DeleteApplication" />.
        /// </summary>
        /// <param name="id">application id</param>
        public DeleteApplication(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
        }

        /// <summary>
        ///     Gets id of the application to delete.
        /// </summary>
        public int Id { get; private set; }
    }
}