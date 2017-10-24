using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Triggers.Commands
{
    /// <summary>
    ///     Delete a trigger
    /// </summary>
    [Message]
    public class DeleteTrigger
    {
        /// <summary>
        ///     Creates a new instance of <see cref="DeleteTrigger" />.
        /// </summary>
        /// <param name="id">primary key</param>
        public DeleteTrigger(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
        }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; private set; }
    }
}